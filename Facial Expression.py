from deepface import DeepFace
import cv2

cap = cv2.VideoCapture(0)  # Use 0 for the default webcam

while True:
    ret, frame = cap.read()

    if ret:
        result = DeepFace.analyze(frame, actions=['emotion'], enforce_detection=False)
        emotion = result[0]['dominant_emotion']

        font = cv2.FONT_HERSHEY_SIMPLEX
        fontScale = 1
        color = (255, 0, 0)

        frame = cv2.putText(frame, "Current Emotion: " + emotion, (10, 50), font, fontScale, color, 1, cv2.LINE_AA)

        cv2.imshow('Demo', frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    else:
        break

cap.release()
cv2.destroyAllWindows()
