import cv2
import mediapipe as mp
import time
from dollarpy import Recognizer, Template, Point

mpPose = mp.solutions.pose
mpDraw = mp.solutions.drawing_utils
pose = mpPose.Pose()
cap = cv2.VideoCapture('PoseVideos/Test/stilltest1.mp4')
prevTime = 0
abTot =0
norTot =0
abCt=0
norCt=0

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
            if id == 23 or id == 25 or id == 27 or id == 24 or id == 26 or id == 28:
                ct += 1
                # outfile.write("Point(" + str(cx) + "," + str(cy) + "," + str(ct) + ")" + ",")
                if id == 23:
                    stroke1.append(Point(cx, cy, ct))
                if id == 24:
                    stroke2.append(Point(cx, cy, ct))
                if id == 25:
                    stroke3.append(Point(cx, cy, ct))
                if id == 26:
                    stroke4.append(Point(cx, cy, ct))
                if id == 27:
                    stroke5.append(Point(cx, cy, ct))
                if id == 28:
                    stroke6.append(Point(cx, cy, ct))
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
#####################################
textToRead = open("normal.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
templateA1 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    templateA1.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
########################################
textToRead = open("normalTemp2.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
tempNormal2 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    tempNormal2.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
#######################################
textToRead = open("normalWalkSide.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalWalkSide =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalWalkSide.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
#########################################
textToRead = open("normalWalkFront.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalWalkFront =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalWalkFront.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
#########################################
textToRead = open("normalMWalkFront.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalMWalkFront =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalMWalkFront.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
#########################################
textToRead = open("normalMWalkSide.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalMWalkSide =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalMWalkSide.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
#########################################
textToRead = open("dazed.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
templateA2 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    templateA2.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()

#######################################
textToRead = open("abnormalDrunk.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
tempAbnormal2 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    tempAbnormal2.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalYDazedWalkSide.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalYDazedWalkSide =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalYDazedWalkSide.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalYDazedWalkFront.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalYDazedWalkFront =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalYDazedWalkFront.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalWDazedWalkSide.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalWDazedWalkSide =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalWDazedWalkSide.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalWDazedWalkFront.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalWDazedWalkFront =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalWDazedWalkFront.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalGait1.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait1 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait1.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalGait2.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait2 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait2.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalGait3.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait3 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait3.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalGait4.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait4 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait4.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################

textToRead = open("abnormalGait5.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait5 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait5.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalGait6.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait6 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait6.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalGait7.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait7 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait7.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalGait8.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait8 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait8.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("abnormalGait9.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
abnormalGait9 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    abnormalGait9.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
#normal = Template('normal', templateA1)
textToRead = open("normalGait1.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait1 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait1.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("normalGait2.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait2 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait2.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("normalGait3.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait3 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait3.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("normalGait4.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait4 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait4.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("normalGait5.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait5 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait5.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("normalGait6.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait6 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait6.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("normalGait7.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait7 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait7.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("normalGait8.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait8 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait8.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("normalGait9.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
normalGait9 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    normalGait9.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("wave1.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
wave1 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    wave1.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("wave2.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
wave2 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    wave2.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("wave3.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
wave3 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    wave3.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################

textToRead = open("wave4.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
wave4 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    wave4.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("wave5.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
wave5 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    wave5.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("wave6.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
wave6 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    wave6.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("still1.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
still1 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    still1.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("still2.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
still2 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    still2.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("still3.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
still3 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    still3.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("still4.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
still4 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    still4.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("still5.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
still5 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    still5.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################
textToRead = open("still6.txt", 'r')
file_content = textToRead.read()
content_list = file_content.split(",")
still6 =[]
a=0
b=1
c=2
for m in range(0,len(content_list),3):
    still6.append(Point(int(content_list[a]),int(content_list[b]),int(content_list[c])))
    a+=3
    b+=3
    c+=3

textToRead.close()
######################################

normalTemp = [[Template('normal', templateA1)], [Template('normal', tempNormal2)],
              [Template('normal', normalWalkSide)], [Template('normal', normalWalkFront)],
              [Template('normal', normalMWalkFront)], [Template('normal', normalMWalkSide)],
              [Template('normal', normalGait1)], [Template('normal', normalGait2)],
              [Template('normal', normalGait3)], [Template('normal', normalGait4)],
              [Template('normal', normalGait5)], [Template('normal', normalGait6)],
              [Template('normal', normalGait7)], [Template('normal', normalGait8)],
              [Template('normal', normalGait9)]]


abnormalTemp = [
    [Template('abnormal', templateA2)], [Template('abnormal', tempAbnormal2)],
    [Template('abnormal', abnormalYDazedWalkFront)], [Template('abnormal', abnormalYDazedWalkSide)],
    [Template('abnormal', abnormalWDazedWalkFront)], [Template('abnormal', abnormalWDazedWalkSide)],
    [Template('abnormal', abnormalGait1)], [Template('abnormal', abnormalGait2)],
    [Template('abnormal', abnormalGait3)], [Template('abnormal', abnormalGait4)],
    [Template('abnormal', abnormalGait5)], [Template('abnormal', abnormalGait6)],
    [Template('abnormal', abnormalGait7)], [Template('abnormal', abnormalGait8)],
    [Template('abnormal', abnormalGait9)]]

waveTemp = [[Template('waving', wave1)], [Template('waving', wave2)],
              [Template('waving', wave3)],  [Template('waving', wave4)],  [Template('waving', wave5)],  [Template('waving', wave6)]]

standingStill = [
    [Template('standingStill', still1)], [Template('standingStill', still2)],
    [Template('standingStill', still3)], [Template('standingStill', still4)], [Template('standingStill', still5)], [Template('standingStill', still6)]]



abTot =0
norTot =0
abCt=0
norCt=0
#dazed = Template('abnormal', templateA2)

# Create a 'Recognizer' object and pass the created 'Template' objects as a list.

#recognizer = Recognizer([normal, dazed])

#recognizer = Recognizer([normalTemp, abnormalTemp])

# Call 'recognize(...)' to match a list of 'Point' elements to the previously defined templates.

'''for x in normalTemp:
    recognizer = Recognizer(x)
    result = recognizer.recognize(allstrokes)
    if result[1] > 0.3:
        norCt += 1
        norTot += result[1]

for y in abnormalTemp:
    recognizer = Recognizer(y)
    result = recognizer.recognize(allstrokes)
    if result[1] > 0.3:
        abCt += 1
        abTot += result[1]'''


for x in waveTemp:
    recognizer = Recognizer(x)
    result = recognizer.recognize(allstrokes)
    if result[1] > 0.1:
        norCt += 1
        norTot += result[1]

for y in standingStill:
    recognizer = Recognizer(y)
    result = recognizer.recognize(allstrokes)
    if result[1] > 0.1:
        abCt += 1
        abTot += result[1]

if abTot > norTot:
    gait = "standingStill"
    percent = abTot*100
    avg = abCt/4
else:
    gait = "wave"
    percent = norTot*100
    avg = norCt/4

print(gait)
#result = recognizer.recognize(allstrokes)
#print(result)