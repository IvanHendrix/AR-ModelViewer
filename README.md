# 🧠 ARModelViewer

**An AR application for viewing 3D models in augmented reality.**  
The user inputs a URL to a `.glb` model — the app downloads it and allows interaction (rotate, scale, move) in AR space.

---

## 📱 APK

🔗 [Download APK](https://drive.google.com/file/d/1DFCa2LJEKS63v_cyLiZvUIhRqMd-F8Eb/view?usp=drive_link)  
*(Download and install on an Android device)*

---

## 🛠 How to Build & Run

1. Open the project in **Unity 6.0.0 (6000.0.43f1)**.
2. Make sure the following modules are installed via Unity Hub:
   - Android Build Support  
   - SDK & NDK  
   - OpenJDK  
3. Go to `Edit → Preferences → External Tools` and verify that all SDK/NDK/JDK paths are correctly set.
4. Go to `File → Build Settings` and switch platform to `Android`.
5. Ensure the first scene in build is `InitScene`.
6. Press **Build** or **Build and Run**.

---

## 🚀 How to Use

- After launching the app on an Android device, allow camera permission.
- Tap **Insert URL** and paste a `.glb` model link (e.g. from Sketchfab, GitHub, etc).
- Tap **Load** — the model will appear in AR space and can be rotated, scaled, and moved.

---

## 🧩 Tech Stack

- Unity 6 + AR Foundation
- Runtime GLB model loading from URL
- Touch input: tap, swipe, pinch gestures
