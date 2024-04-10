import socket
import threading
import csv
import datetime
import os

# Define base filename
base_filename = 'data_log_reset'

# Check if the file exists and generate a unique filename
timestamp_str = datetime.datetime.now().strftime('%Y%m%d_%H%M%S')
filename = f"{base_filename}_{timestamp_str}.csv"

# Open a new CSV file to log the data
csv_file = open(filename, 'w', newline='', buffering=1)
csv_writer = csv.writer(csv_file)
csv_writer.writerow(['Message', 'Relative Time (ms)'])  # Header row

# Setting up sockets
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock_pt = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

ip_addr = "192.168.0.3"  # PC IP

# Binding sockets
sock.bind((ip_addr, 5555))  # For Android
sock_pt.bind((ip_addr, 5556))  # For point tracking
unity_sock.bind(("127.0.0.1", 5560))  # Unity receiver

initial_timestamp = None  # Will hold the timestamp of the first data received or reset

def reset_initial_timestamp():
    global initial_timestamp
    initial_timestamp = round(datetime.datetime.now().timestamp() * 1000)

def get_relative_ms_timestamp():
    current_timestamp = round(datetime.datetime.now().timestamp() * 1000)
    if initial_timestamp is None:
        reset_initial_timestamp()
    return current_timestamp - initial_timestamp

def check_for_reset(msg):
    if '[' in msg:
        reset_initial_timestamp()

def watchToUnity():
    while True:
        data, addr = sock.recvfrom(1024)
        msg = str(data, 'utf-8')
        if msg:
            print(msg)
            check_for_reset(msg)  # Check if message requires timer reset
            relative_timestamp = get_relative_ms_timestamp()
            csv_writer.writerow([msg, relative_timestamp])
        unity_sock.sendto(data, ("127.0.0.1", 5556))  # Send to Unity

def pointToUnity():
    while True:
        data, addr = sock_pt.recvfrom(1024)
        msg = str(data, 'utf-8')
        if msg:
            print(msg)
            check_for_reset(msg)  # Check if message requires timer reset
            relative_timestamp = get_relative_ms_timestamp()
            csv_writer.writerow([msg, relative_timestamp])
        unity_sock.sendto(data, ("127.0.0.1", 5566))  # Send to Unity

# Starting threads
thread1 = threading.Thread(target=watchToUnity)
thread2 = threading.Thread(target=pointToUnity)
thread1.start()
thread2.start()

# Make sure to properly close the CSV file when the script is stopped
import atexit

def close_csv_file():
    csv_file.close()

atexit.register(close_csv_file)
