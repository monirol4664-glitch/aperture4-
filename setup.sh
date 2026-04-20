#!/bin/bash

echo "🚀 Setting up Android APK project with GitHub Actions..."

# Create solution
dotnet new sln -n MyAndroidApp

# Create MAUI app
dotnet new maui -n MyAndroidApp -o src/MyAndroidApp

# Add to solution
dotnet sln MyAndroidApp.sln add src/MyAndroidApp/MyAndroidApp.csproj

# Create GitHub workflows directory
mkdir -p .github/workflows

# Create initial git repository
git init
git add .
git commit -m "Initial commit: Android app with GitHub Actions"

echo ""
echo "✅ Project setup complete!"
echo ""
echo "Next steps:"
echo "1. Create a repository on GitHub"
echo "2. Push your code:"
echo "   git remote add origin https://github.com/YOUR_USERNAME/YOUR_REPO.git"
echo "   git branch -M main"
echo "   git push -u origin main"
echo ""
echo "3. Go to Actions tab on GitHub to see your APK being built"
echo "4. Download APK from Artifacts section when build completes"