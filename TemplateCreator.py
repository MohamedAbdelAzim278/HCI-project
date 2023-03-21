import cv2
import mediapipe as mp
import time
from dollarpy import Recognizer, Template, Point
#mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_hands = mp.solutions.hands

mpPose = mp.solutions.pose
mpDraw = mp.solutions.drawing_utils

pose = mpPose.Pose()
cap = cv2.VideoCapture('Emotions/still6.mp4')
prevTime = 0

outfile = open('still6.txt', 'w')

stroke1 = []
stroke2 = []
stroke3 = []
stroke4 = []
stroke5 = []
stroke6 = []

while cap.isOpened():
    success, img = cap.read()
    if not success:
        break
    imgRGB = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
    results = pose.process(imgRGB)

    if results.pose_landmarks:
        mpDraw.draw_landmarks(img, results.pose_landmarks, mpPose.POSE_CONNECTIONS)
        ct = 0
        for id, lm in enumerate(results.pose_landmarks.landmark):
            h, w, c = img.shape
            # print(id, lm, lm)
            cx, cy = int(lm.x * w), int(lm.y * h)
            cv2.circle(img, (cx, cy), 10, (255, 0, 255), cv2.FILLED)
            if id == 15 or id == 16 or id == 17 or id == 18 or id == 19 or id == 20 or id == 21 or id == 22:
                ct += 1
                # outfile.write("Point(" + str(cx) + "," + str(cy) + "," + str(ct) + ")" + ",")
                if id == 15:
                    stroke1.append(cx)
                    stroke1.append(cy)
                    stroke1.append(ct)
                if id == 16:
                    stroke2.append(cx)
                    stroke2.append(cy)
                    stroke2.append(ct)
                if id == 17:
                    stroke3.append(cx)
                    stroke3.append(cy)
                    stroke3.append(ct)
                if id == 18:
                    stroke4.append(cx)
                    stroke4.append(cy)
                    stroke4.append(ct)
                if id == 19:
                    stroke5.append(cx)
                    stroke5.append(cy)
                    stroke5.append(ct)
                if id == 20:
                    stroke6.append(cx)
                    stroke6.append(cy)
                    stroke6.append(ct)
                if id == 21:
                    stroke6.append(cx)
                    stroke6.append(cy)
                    stroke6.append(ct)
                if id == 22:
                    stroke6.append(cx)
                    stroke6.append(cy)
                    stroke6.append(ct)
                    ct = 0


    currTime = time.time()
    fps = 1/(currTime - prevTime)
    prevTime = currTime

    cv2.putText(img, str(int(fps)), (70, 200), cv2.FONT_HERSHEY_SIMPLEX, 3, (255, 255, 255), 3)
    cv2.imshow("Image", img)
    cv2.waitKey(1)


allstrokes =[]
allstrokes.extend(stroke1)
allstrokes.extend(stroke2)
allstrokes.extend(stroke3)
allstrokes.extend(stroke4)
allstrokes.extend(stroke5)
allstrokes.extend(stroke6)

for x in allstrokes:
    #if x != allstrokes[-1]:
    outfile.write(str(x) + ",")
    #else:
        #outfile.write(str(x))

outfile.close()