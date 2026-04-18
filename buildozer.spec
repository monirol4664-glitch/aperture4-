[app]

# (str) Title of your application
title = Python Console

# (str) Package name
package.name = pythonconsole

# (str) Package domain (needs to be unique)
package.domain = org.pythonapp

# (str) Source code directory
source.dir = .

# (str) Version of your application
version = 1.0.0

# (str) Version code (integer, auto-incrementing)
version.code = 1

# (list) Source files to include
source.include_exts = py,png,jpg,kv,atlas,ttf,txt,json,js,html,css

# (list) Application requirements
requirements = python3,kivy

# (str) Supported orientation (portrait, landscape, all)
orientation = portrait

# (bool) Fullscreen mode
fullscreen = 0

# (str) Android API level
android.api = 30

# (int) Android SDK version
android.sdk = 30

# (str) Android NDK version
android.ndk = 23b

# (bool) Allow building with debug APK
android.allow_backup = True

# (list) Permissions
android.permissions = WRITE_EXTERNAL_STORAGE,INTERNET

# (bool) Enable/disable debug mode
android.debug = True

# (str) Log level (0=debug, 1=info, 2=warning, 3=error)
log_level = 2

# (bool) Warn if buildozer is run as root
warn_on_root = 1

# (str) Presplash file (image to show before app loads)
# presplash.filename = %(source.dir)s/presplash.png

# (str) Icon file
# icon.filename = %(source.dir)s/icon.png

# (list) Permissions for Android
android.permissions = WRITE_EXTERNAL_STORAGE

# (list) Android manifest metadata
# android.manifest_metadata =

# (bool) Enable/disable ARMv7 support
android.arch = arm64-v8a, armeabi-v7a
