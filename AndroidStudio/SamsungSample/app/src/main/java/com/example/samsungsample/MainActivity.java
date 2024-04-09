    package com.example.samsungsample;

    import android.app.Activity;
    import android.content.Intent;
    import android.graphics.Color;
    import android.hardware.Sensor;
    import android.hardware.SensorEvent;
    import android.hardware.SensorEventListener;
    import android.hardware.SensorManager;
    import android.os.Bundle;
    import android.util.Log;
    import android.view.WindowManager;
    import android.widget.Button;

    import com.example.samsungsample.databinding.ActivityMainBinding;

    import java.io.IOException;
    import java.net.DatagramPacket;
    import java.net.DatagramSocket;
    import java.net.InetAddress;
    import java.net.SocketException;
    import java.net.UnknownHostException;
    import java.nio.charset.StandardCharsets;

    public class MainActivity extends Activity implements SensorEventListener {

        private ActivityMainBinding binding;
        private float[] accelerometerReading = new float[3];
        private float[] magnetometerReading = new float[3];
        private float[] mRotationMatrix = new float[9];
        private float[] orientation = new float[3];
        static int[] mid_point= {225,225};
        public static boolean wifi_found = false;

        static String ip="";

        protected void onCreate(Bundle savedInstanceState) {
            super.onCreate(savedInstanceState);
            binding = ActivityMainBinding.inflate(getLayoutInflater());
            setContentView(binding.getRoot());

            keepScreenOn();

            Button auth_btn = findViewById(R.id.AUTH);
            auth_btn.setOnClickListener(v -> startActivity(new Intent(MainActivity.this, AUTH.class)));

            discoverWifi();

            SensorManager mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
            mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_NORMAL);
            mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_MAGNETIC_FIELD), SensorManager.SENSOR_DELAY_NORMAL);
        }

        private void keepScreenOn() {
            getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
        }

        private void discoverWifi() {
            new Thread(() -> {
                while (!wifi_found) {
                    try {
                        ip=InetAddress.getLocalHost().getHostAddress();
                    wifi_found=true;
                    Log.d("IUIP",ip);
                    new Thread(() -> {
                        sendData(ip+",",5590);
                    }).start();
                    } catch (UnknownHostException e) {
                        Log.d("Error", "Error");
                    }
                }
            }).start();
        }

        public static void sendData(String message, int port) {
            try {
                DatagramSocket socket = new DatagramSocket();
                InetAddress address = InetAddress.getByName("192.168.1.32"); // Ensure this is the correct IP
                byte[] buf = message.getBytes(StandardCharsets.UTF_8); // Explicitly specify UTF-8
                DatagramPacket packet = new DatagramPacket(buf, buf.length, address, port);
                socket.send(packet);
                System.out.println("Message sent: " + message);
                socket.close();
            } catch (Exception e) {
                System.out.println("Error: " + e.getMessage());
            }
        }



        public static void recvData() {
            try {
                DatagramSocket udpsocket = new DatagramSocket(5555); // Specify the port
                byte[] buffer = new byte[100];
                DatagramPacket packet = new DatagramPacket(buffer, buffer.length);
                udpsocket.receive(packet);
                String recv_msg = new String(packet.getData(), 0, packet.getLength());
                Log.d("RECV", recv_msg);
                udpsocket.close();
            } catch (IOException e) {
                e.printStackTrace();
            }
        }


        @Override
        public void onSensorChanged(SensorEvent event) {
            if (event.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
                System.arraycopy(event.values, 0, accelerometerReading, 0, accelerometerReading.length);
            } else if (event.sensor.getType() == Sensor.TYPE_MAGNETIC_FIELD) {
                System.arraycopy(event.values, 0, magnetometerReading, 0, magnetometerReading.length);
            }
            updateOrientationAngles();
        }

        private void updateOrientationAngles() {
            SensorManager.getRotationMatrix(mRotationMatrix, null, accelerometerReading, magnetometerReading);
            SensorManager.getOrientation(mRotationMatrix, orientation);
        }

        @Override
        public void onAccuracyChanged(Sensor sensor, int accuracy) {
            // Not used, but required for the SensorEventListener interface
        }
    }
