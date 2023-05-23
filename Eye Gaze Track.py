import cv2
import mediapipe as mp
import numpy as np

map_face_mesh = mp.solutions.face_mesh
cap = cv2.VideoCapture(0)  # Use default camera (change to appropriate camera index if needed)

RightEyeRight = [33]
RightEyeLeft = [133]
LeftEyeRight = [362]
LeftEyeLeft = [263]
LeftIris = [474, 475, 476, 477]
RightIris = [469, 470, 471, 472]

lmposition = []
flag, ctright, ctleft = 0, 0, 0

with map_face_mesh.FaceMesh(
        max_num_faces=1,
        refine_landmarks=True,
        min_detection_confidence=0.5,
        min_tracking_confidence=0.5
) as face_mesh:

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        frame_height, frame_width = frame.shape[:2]
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_RGB2BGR)
        results = face_mesh.process(rgb_frame)

        h, w, c = frame.shape

        if results.multi_face_landmarks:
            facepoints = np.array([np.multiply([p.x, p.y], [w, h]).astype(int)
                                   for p in results.multi_face_landmarks[0].landmark])
            (cx_left, cy_left), radius_left = cv2.minEnclosingCircle(facepoints[LeftIris])
            (cx_right, cy_right), radius_right = cv2.minEnclosingCircle(facepoints[RightIris])
            center_left = np.array([cx_left, cy_left], dtype=np.int32)
            center_right = np.array([cx_right, cy_right], dtype=np.int32)
            cv2.circle(frame, center_left, int(radius_left), (0, 0, 255), 1, cv2.LINE_AA)
            cv2.circle(frame, center_right, int(radius_right), (0, 0, 255), 1, cv2.LINE_AA)

            distance_half_left = np.linalg.norm(center_left - facepoints[LeftEyeRight])
            distance_all_left = np.linalg.norm(facepoints[LeftEyeLeft] - facepoints[LeftEyeRight])
            ratio_left = distance_half_left / distance_all_left

            distance_half_right = np.linalg.norm(center_right - facepoints[RightEyeRight])
            distance_all_right = np.linalg.norm(facepoints[RightEyeLeft] - facepoints[RightEyeRight])
            ratio_right = distance_half_right / distance_all_right

            if ratio_left <= 0.4 and ratio_right <= 0.4:
                position = 'Both eyes right'
                flag = 1
                ctright += 1
            elif ratio_left > 0.6 and ratio_right > 0.6:
                position = 'Both eyes left'
                flag = 1
                ctleft += 1
            elif ratio_left <= 0.4 and ratio_right > 0.6:
                position = 'Left eye right, right eye left'
                flag = 1
                ctright += 1
                ctleft += 1
            else:
                position = 'Center'
                flag = 0

            print(position)
            lmposition.append(position)

        cv2.putText(frame, 'right Eye', (50, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2, cv2.LINE_AA)
        cv2.putText(frame, position, (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2, cv2.LINE_AA)
        cv2.imshow('frame', frame)

        key = cv2.waitKey(1)
        if key == ord('q') or key == ord('Q'):
            break

    cv2.destroyAllWindows()
    cap.release()
    print(lmposition)

if ctright > 10 or ctleft > 10:
    print('Bad Eye Contact')
