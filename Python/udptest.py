import socket
import threading
sock=socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
sock_pt=socket.socket(socket.AF_INET,socket.SOCK_DGRAM)

ip_addr="192.168.1.32"#"

sock.bind((ip_addr,5555)) #android
sock_pt.bind((ip_addr,5556))

unity_sock=socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
unity_sock.bind(("127.0.0.1",5560))



def watchToUnity():
    while(True):
        data,addr=sock.recvfrom(1024)
        msg=str(data,'utf-8')
        if(msg!=""):print(msg)
        unity_sock.sendto(data,("127.0.0.1",5556)) #unity
        

def pointToUnity():
    while(True):
        #print(sock_pt)
        data,addr=sock_pt.recvfrom(1024)
        #print(data)
        msg=str(data,'utf-8')
        if(msg!=""):print(msg)
        #msg_arr=msg.split(",")
        unity_sock.sendto(data,("127.0.0.1",5566))
        


thread1=threading.Thread(target=watchToUnity)
thread2=threading.Thread(target=pointToUnity)
thread1.start()
thread2.start()


