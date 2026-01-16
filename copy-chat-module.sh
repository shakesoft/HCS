#!/bin/bash

# Script to copy Chat module from chat-samples to src with namespace conversion

SOURCE_DIR="chat-samples/src"
TARGET_DIR="src"

# Function to copy and convert namespace
copy_and_convert() {
    local source_file=$1
    local target_file=$2
    
    # Create target directory if it doesn't exist
    mkdir -p "$(dirname "$target_file")"
    
    # Copy file and convert namespace
    sed 's/Volo\.Chat/HC.Chat/g' "$source_file" > "$target_file"
    
    echo "Copied: $source_file -> $target_file"
}

# Copy Domain layer
echo "Copying Domain layer..."
find "$SOURCE_DIR/Volo.Chat.Domain" -name "*.cs" -type f | while read file; do
    target=$(echo "$file" | sed "s|$SOURCE_DIR/Volo.Chat.Domain|$TARGET_DIR/HC.Domain/Chat|")
    copy_and_convert "$file" "$target"
done

# Copy Application layer
echo "Copying Application layer..."
find "$SOURCE_DIR/Volo.Chat.Application" -name "*.cs" -type f | while read file; do
    target=$(echo "$file" | sed "s|$SOURCE_DIR/Volo.Chat.Application|$TARGET_DIR/HC.Application/Chat|")
    copy_and_convert "$file" "$target"
done

# Copy Application Contracts layer
echo "Copying Application Contracts layer..."
find "$SOURCE_DIR/Volo.Chat.Application.Contracts" -name "*.cs" -type f | while read file; do
    target=$(echo "$file" | sed "s|$SOURCE_DIR/Volo.Chat.Application.Contracts|$TARGET_DIR/HC.Application.Contracts/Chat|")
    copy_and_convert "$file" "$target"
done

# Copy EF Core layer
echo "Copying EF Core layer..."
find "$SOURCE_DIR/Volo.Chat.EntityFrameworkCore" -name "*.cs" -type f | while read file; do
    target=$(echo "$file" | sed "s|$SOURCE_DIR/Volo.Chat.EntityFrameworkCore|$TARGET_DIR/HC.EntityFrameworkCore/Chat|")
    copy_and_convert "$file" "$target"
done

# Copy HTTP API layer
echo "Copying HTTP API layer..."
find "$SOURCE_DIR/Volo.Chat.HttpApi" -name "*.cs" -type f | while read file; do
    target=$(echo "$file" | sed "s|$SOURCE_DIR/Volo.Chat.HttpApi|$TARGET_DIR/HC.HttpApi/Chat|")
    copy_and_convert "$file" "$target"
done

echo "Done! Please review the converted files and fix any remaining issues."
