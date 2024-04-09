package com.example.samsungsample;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.util.AttributeSet;
import android.util.Log;
import android.view.View;

import androidx.annotation.Nullable;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class AUTH_View extends View {
    int area_num = 0;
    boolean center_touched = false;
    private List<String> elements;
    public List<String> secondLevelElements = new ArrayList<>();
    Vibrator v;
    Paint paint;
    private final int INNER_CIRCLE_DIAMETER = 330;
    final int OK_POSITION = 9;
    final int BACK_POSITION = 10;
    private final int[] angles = {90, 180, 270, 0};
    private final int[] num_angles = {45, 135, 225, 315};


    public AUTH_View(Context context) {
        super(context);
        v = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        init();
    }

    public AUTH_View(Context context, @Nullable AttributeSet attrs) {
        super(context, attrs);
        v = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        init();
    }

    private void init() {
        paint = new Paint();
        paint.setAntiAlias(true);
        paint.setTextSize(40);
        paint.setTextAlign(Paint.Align.CENTER);
        paint.setColor(Color.WHITE);
        paint.setStrokeWidth(100);

        elements = new ArrayList<>();
        for (int i = 0; i < 10; i++) {
            elements.add(String.valueOf(i));
        }

        Collections.shuffle(elements);

        elements.add(OK_POSITION, "OK");
        elements.add(BACK_POSITION, "<-");
        new Thread(() -> MainActivity.sendData(elements.toString(), 5555)).start();
    }


    public void onOKSelected(){
        elements = new ArrayList<>();
        for (int i = 0; i < 10; i++) {
            elements.add(String.valueOf(i));
        }

        Collections.shuffle(elements);

        elements.add(OK_POSITION, "OK");
        elements.add(BACK_POSITION, "<-");
        new Thread(() -> MainActivity.sendData(elements.toString(), 5555)).start();
    }





    public void showInitialElements() {
        center_touched = false;
        invalidate();
    }

    public int getBezelArea(float x, float y) {
        double ux = x - MainActivity.mid_point[0];
        double uy = y - MainActivity.mid_point[1];
        double dist = Math.sqrt(Math.pow(ux, 2) + Math.pow(uy, 2));
        double cos = (-uy) / dist;
        double angle = Math.acos(cos);
        angle = angle * (180.0f) / Math.PI;

        if (x < MainActivity.mid_point[0]) {
            angle = 360 - angle;
        }

        if (dist > 160) {
            int area = ((int) (angle) / 90) % 4;
            area_num = area;
            return area;
        } else {
            return -1;
        }
    }

    public void vibrate() {
        v.vibrate(VibrationEffect.createOneShot(100, VibrationEffect.DEFAULT_AMPLITUDE));
    }


    public void updateSecondLevel(int quadrant) {
        secondLevelElements.clear();
        vibrate();

        int emptyIndex;
        switch (quadrant) {
            case 0:
                emptyIndex = 0;
                break;
            case 1:
                emptyIndex = 1;
                break;
            case 2:
                emptyIndex = 2;
                break;
            case 3:
                emptyIndex = 3;
                break;
            default:
                emptyIndex = 0;
        }
        for (int i = 0; i < 3; i++) {
            int elementIndex = (quadrant * 3 + i) % elements.size();
            if (elementIndex == 0) {
                elementIndex = 12;
            }
            secondLevelElements.add(elements.get(elementIndex - 1));
        }
        secondLevelElements.add(emptyIndex, " ");
        center_touched = true;
        invalidate();
    }


    public boolean isTouchInCenterRegion(float x, float y) {
        int width = getWidth();
        int height = getHeight();
        float centerX = width / 2.0f;
        float centerY = height / 2.0f;
        float regionRadius = INNER_CIRCLE_DIAMETER / 2.0f;
        float dx = x - centerX;
        float dy = y - centerY;
        float distanceSquared = dx * dx + dy * dy;
        float radiusSquared = regionRadius * regionRadius;

        return distanceSquared < radiusSquared;
    }

    public void resetToInitialState() {
        secondLevelElements.clear();
        showInitialElements();
    }

    private void drawCircleWithText(Canvas canvas, int circleColor, boolean drawText, String text, int textColor, int textSize) {
        int width = getWidth();
        int height = getHeight();
        float centerX = width / 2.0f;
        float centerY = height / 2.0f;
        float radius = (float) INNER_CIRCLE_DIAMETER / 2;

        Paint paint = new Paint();
        paint.setColor(circleColor);
        paint.setStyle(Paint.Style.FILL);

        // Draw the main circle
        canvas.drawCircle(centerX, centerY, radius, paint);

        if (drawText) {
            paint.setColor(textColor);
            paint.setTextSize(textSize);
            paint.setTextAlign(Paint.Align.CENTER);

            // Adjust the y position to center the text vertically
            float textY = centerY + (paint.descent() - paint.ascent()) / 2 - paint.descent();
            canvas.drawText(text, centerX, textY, paint);
        }
    }





    public void onDraw(Canvas canvas) {

        paint.setColor(Color.GRAY);
        paint.setStrokeWidth(3);
        float centerX = getWidth() / 2f;
        float centerY = getHeight() / 2f;
        float radius = Math.min(centerX, centerY) - 28;

        if (!center_touched) {
            drawCircleWithText(canvas, Color.GRAY, false, null, 0, 0);
            for (int i = 0; i < elements.size(); i++) {
                double angle = (2 * Math.PI * i / elements.size()) + Math.toRadians(45);
                float x = (float) (centerX + radius * Math.sin(angle));
                float y = (float) (10 + centerY - radius * Math.cos(angle));
                canvas.drawText(elements.get(i), x, y, paint);
            }

            for (int i = 0; i < 4; i++) {
                double angle = Math.toRadians(num_angles[i]) + Math.toRadians(45);
                float startX = (float) (centerX + radius * Math.sin(angle));
                float startY = (float) (centerY - radius * Math.cos(angle));
                float endX = (float) (centerX + radius * 5 * Math.sin(angle + Math.PI));
                float endY = (float) (centerY - radius * 5 * Math.cos(angle + Math.PI));
                canvas.drawLine(startX, startY, endX, endY, paint);
            }

        } else {

            for (int i = 0; i < secondLevelElements.size(); i++) {
                double angle = Math.toRadians(num_angles[i]);
                float x = (float) (centerX + radius * Math.sin(angle));
                float y = (float) (centerY - radius * Math.cos(angle));
                canvas.drawText(secondLevelElements.get(i), x, y, paint);
            }

            paint.setColor(Color.GRAY);
            paint.setStrokeWidth(3);

            for (int i = 0; i < 4; i++) {
                double angle = Math.toRadians(angles[i]);
                float startX = (float) (centerX + radius * Math.sin(angle));
                float startY = (float) (centerY - radius * Math.cos(angle));
                float endX = (float) (centerX + radius * 5 * Math.sin(angle + Math.PI));
                float endY = (float) (centerY - radius * 5 * Math.cos(angle + Math.PI));
                canvas.drawLine(startX, startY, endX, endY, paint);
            }
            drawCircleWithText(canvas, Color.RED, true, "Cancel", Color.WHITE, 30);
        }

    }


}