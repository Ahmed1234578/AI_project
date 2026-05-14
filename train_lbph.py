import cv2
import os
import numpy as np

dataset_path = "dataset"

faces = []
labels = []
label_map = {}

label_id = 0

# Face detector
face_detector = cv2.CascadeClassifier(
    cv2.data.haarcascades + 'haarcascade_frontalface_default.xml'
)

for person_name in os.listdir(dataset_path):

    person_path = os.path.join(dataset_path, person_name)

    label_map[label_id] = person_name

    for image_name in os.listdir(person_path):

        image_path = os.path.join(person_path, image_name)

        img = cv2.imread(image_path)

        if img is None:
            continue

        # Convert to grayscale
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

        # Improve contrast
        gray = cv2.equalizeHist(gray)

        # Reduce noise
        gray = cv2.GaussianBlur(gray, (5,5), 0)

        faces_rect = face_detector.detectMultiScale(gray, 1.3, 5)

        for (x, y, w, h) in faces_rect:

            face = gray[y:y+h, x:x+w]

            face = cv2.resize(face, (200, 200))

            faces.append(face)
            labels.append(label_id)

    label_id += 1

# Create LBPH recognizer
model = cv2.face.LBPHFaceRecognizer_create()

# Train model
model.train(faces, np.array(labels))

# Save model
model.save("lbph_model.yml")

print("Model Trained Successfully ✅")