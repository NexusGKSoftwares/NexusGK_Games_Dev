#!/bin/bash
# Quick script to check if Unity scripts have basic syntax issues

echo "Checking Unity C# scripts..."
echo "=============================="

# Find all C# scripts
scripts=$(find Assets/Scripts -name "*.cs" 2>/dev/null)

if [ -z "$scripts" ]; then
    echo "No C# scripts found!"
    exit 1
fi

count=0
for script in $scripts; do
    echo -n "Checking $script... "
    
    # Basic checks
    if grep -q "using UnityEngine" "$script"; then
        if grep -q "class" "$script"; then
            echo "✓ OK"
            ((count++))
        else
            echo "✗ Missing class definition"
        fi
    else
        echo "✗ Missing UnityEngine import"
    fi
done

echo ""
echo "Checked $count scripts"
echo ""
echo "To run this game:"
echo "1. Install Unity Hub from https://unity.com/download"
echo "2. Install Unity Editor 2022.3.0 or later"
echo "3. Open Unity Hub and click 'Add' project"
echo "4. Select this folder: $(pwd)"
echo "5. Click 'Open' and Unity will open your project"

