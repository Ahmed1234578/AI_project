import cv2
import os

# =========================
# Load Trained EigenFaces Model
# =========================
model = cv2.face.EigenFaceRecognizer_create()
model.read("eigenface_model.yml")

# =========================
# Create Label Map
# =========================
dataset_path = "dataset"

label_map = {}
label_id = 0

for person_name in os.listdir(dataset_path):
    label_map[label_id] = person_name
    label_id += 1

# =========================
# Load Haar Cascade
# =========================
face_detector = cv2.CascadeClassifier(
    cv2.data.haarcascades + 'haarcascade_frontalface_default.xml'
)

# =========================
# Open Camera
# =========================
cap = cv2.VideoCapture(0)

while True:

    # Read frame
    ret, frame = cap.read()

    if not ret:
        break

    # =========================
    # Pre-processing
    # =========================
    # Convert to grayscale
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    # Improve contrast
    gray = cv2.equalizeHist(gray)

    # Reduce noise
    gray = cv2.GaussianBlur(gray, (5,5), 0)

    # =========================
    # Face Detection
    # =========================
    faces = face_detector.detectMultiScale(
        gray,
        scaleFactor=1.3,
        minNeighbors=5
    )

    for (x, y, w, h) in faces:

        # Crop face
        face = gray[y:y+h, x:x+w]

        # Resize
        face = cv2.resize(face, (200, 200))

        # =========================
        # Matching
        # =========================
        label, confidence = model.predict(face)

        # Confidence threshold
        if confidence < 5000:
            name = label_map[label]
        else:
            name = "Unknown"

        # Draw rectangle
        cv2.rectangle(
            frame,
            (x, y),
            (x+w, y+h),
            (255, 0, 0),
            2
        )

        # Put text
        cv2.putText(
            frame,
            f"{name}",
            (x, y-10),
            cv2.FONT_HERSHEY_SIMPLEX,
            1,
            (255, 0, 0),
            2
        )

    # Show frame
    cv2.imshow("EigenFaces Recognition", frame)

    # ESC to exit
    if cv2.waitKey(1) == 27:
        break

# Release resources
cap.release()
cv2.destroyAllWindows()