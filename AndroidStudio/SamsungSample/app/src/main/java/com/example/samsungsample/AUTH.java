package com.example.samsungsample;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.MotionEvent;

import java.util.ArrayList;
import java.util.List;

public class AUTH extends Activity {

    float start_x = 0;
    float start_y = 0;
    AUTH_View auth_view;
    boolean secondLevelUpdated = false;
    boolean  initialTouchInBezel = false;
    boolean touchedCenter = false;
    int initialQuadrant = -1;
    private List<String> selectedElementsList = new ArrayList<>();


    String ip = "192.168.1.32";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        auth_view = new AUTH_View(this);
        setContentView(auth_view);
    }

    @Override
    public boolean onTouchEvent(MotionEvent event) {
        int x = (int) event.getX();
        int y = (int) event.getY();

        switch (event.getAction()) {
            case MotionEvent.ACTION_DOWN:
                start_x = x;
                start_y = y;
                secondLevelUpdated = false;
                initialTouchInBezel = auth_view.getBezelArea(x, y) != -1;
                initialQuadrant = auth_view.getBezelArea(x, y);
                return true;

            case MotionEvent.ACTION_MOVE:
                new Thread(() -> {
                    MainActivity.sendData("p," + String.valueOf(x) + "," + String.valueOf(y), 5556);

                }).start();
                Log.d("pointValue", String.valueOf(x));
                touchedCenter = auth_view.isTouchInCenterRegion(x,y);
                auth_view.center_touched = touchedCenter;
                if (auth_view.getBezelArea(x, y) != -1){
                    initialQuadrant = auth_view.getBezelArea(x, y);
                }
                if (initialTouchInBezel && !secondLevelUpdated) {
                    if (initialQuadrant != -1 && touchedCenter) {
                        auth_view.updateSecondLevel(initialQuadrant);
                        new Thread(() -> MainActivity.sendData("quadrant"+ initialQuadrant, 5555)).start();
                        secondLevelUpdated = true;
                    }
                    auth_view.invalidate();
                }
                return true;


            case MotionEvent.ACTION_UP:

                if(auth_view.isTouchInCenterRegion(x, y) && secondLevelUpdated){

                    new Thread(() -> MainActivity.sendData("cancel", 5555)).start();
                }
                if (initialQuadrant == -1 && touchedCenter){
                    auth_view.resetToInitialState();
                    touchedCenter = false;

                }
                if (auth_view.isTouchInCenterRegion(x, y)) {
                    auth_view.resetToInitialState();

                } else if (secondLevelUpdated) {
                    float endX = event.getX();
                    float endY = event.getY();
                    int selectedQuadrant = auth_view.getBezelArea(endX, endY);
                    if (selectedQuadrant != -1) {
                        String selectedElement = auth_view.secondLevelElements.get(selectedQuadrant);

                        //no selection
                        if (" ".equals(selectedElement)) {
                            new Thread(() -> MainActivity.sendData("cancel", 5555)).start();
                        } else {
                            //Backspace
                            if ("<-".equals(selectedElement)) {
                                if (!selectedElementsList.isEmpty()) {
                                    selectedElementsList.remove(selectedElementsList.size() - 1);
                                }
                            }
                            //OK
                            if ("OK".equals(selectedElement)) {
                                auth_view.onOKSelected();

                            } else {
                                //Anything else
                                selectedElementsList.add(selectedElement);
                                new Thread(() -> MainActivity.sendData(selectedElement, 5555)).start();

                                Log.d("sent", selectedElement);
                            }
                        }


                        Log.d("SelectedList", selectedElementsList.toString());
                    }

                }
                auth_view.resetToInitialState();
                touchedCenter = false;
                initialTouchInBezel = false;
                secondLevelUpdated = false;

                return true;

        }
        return super.onTouchEvent(event);
    }

}
