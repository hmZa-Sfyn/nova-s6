<h1 align="center">🚀 Nova-S6</h1>
<h2 align="center">Integrated Shell Environment</h2>

<div align="center">
  
[![Version](https://img.shields.io/badge/Version-4.2.yz.24-blue.svg)](https://example.com)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](https://example.com)
[![Status](https://img.shields.io/badge/Status-Active-success.svg)](https://example.com)

</div>

## 🌟 Overview

Nova-S6 is a powerful Integrated Shell Environment that combines multiple specialized shells, extensive file management capabilities, and advanced networking features. Built for both power users and developers, it offers a seamless experience across different computing environments.

## 🛠️ Featured Shells

### 1. USH (Utility Shell)
A lightweight, powerful utility shell designed for everyday tasks and system management. Perfect for users who need quick access to common system operations.

### 2. HQSH (High-Quality Shell)
An advanced system utilities library that powers both the HSH-shell and USH-shell, providing robust system-level operations and enhanced functionality.

### 3. HOLY-C Shell
Inspired by Terry A. Davis's work, this shell brings the power and elegance of the Holy-C language to your terminal environment. Perfect for those who appreciate both functionality and programming artistry.

## 💻 Core Features

### Command Categories

#### Essential Operations
```bash
# Echo Commands
echo      # Echo text without newlines
echoln    # Echo text with each argument on a new line

# Mind Storage Commands
mind -add item1 item2 item3    # Add items to storage
mind -get 1                    # Get item at index 1
mind -list                     # Show all stored items

# Register Storage Commands
reg>>set>>key>>value     # Set a key-value pair
reg>>get>>key            # Get value for a key
reg>>list                # List all registered key-value pairs

# Directory Navigation
cd directory_path      # Change directory

# Environment Variables
$$env::variableName           # Get value of specific variable
$$env>>variableName>>value    # Set environment variable
$$env>>$all                   # List all environment variables
$$env>>$rem>>variableName     # Remove environment variable

macro add alias command     # To add a new macro
macro remove alias command  # To remove a macro
macro update alias command  # To update an existing macro
macro list                  # To list all macros
macro restore               # To restore the deleted macros

exit # To exit the application
```

#### File Operations
```bash
file create filename content  # Create new file with content
file read filename           # Display file content
file delete filename         # Delete specified file
file append filename content # Append content to file
file rename oldname newname  # Rename file
file copy source destination # Copy file
file info filename          # Show file information
file exists filename        # Check if file exists
file lines-count filename   # Count lines in file
file search filename text   # Search for text in file
file encrypt filename       # Encrypt file
file decrypt filename       # Decrypt file
file compress filename      # Compress file
file decompress filename    # Decompress file
file hash filename          # Generate SHA-256 hash
file watch filename         # Monitor file for changes
file temp [content]         # Create temporary file
```

#### Directory Management
```bash
dir create dirname            # Create new directory
dir list dirname             # List directory contents
dir delete dirname           # Delete directory
dir rename oldname newname   # Rename directory
dir exists dirname           # Check if directory exists
dir info dirname             # Show directory information
dir size dirname             # Calculate directory size
dir count-files dirname      # Count files in directory
dir count-dirs dirname       # Count subdirectories
dir backup dirname           # Create directory backup
dir clean dirname            # Remove all contents
dir find dirname pattern     # Find files matching pattern
dir monitor dirname          # Monitor directory changes
```

#### Network Operations
```bash
# TCP Operations
net tcp-connect hostname port [message]  # Establish TCP connection
net tcp-server port                      # Create TCP server

# Email Operations
net smtp-send server port username password to subject body  # Send email

# Security Checks
net rdp-check hostname                   # Check RDP port accessibility
net ssl-check hostname                   # Check SSL certificate

# Basic Network Operations
net ping hostname                        # Test connectivity
net gethost hostname                     # Get host information
net getip hostname                       # Resolve domain name to IP
net scan-ports hostname startPort endPort # Scan port range
net traceroute hostname                  # Trace route to destination
net whois domain                         # Perform WHOIS lookup

# Network Monitoring
net listen port                          # Create network listener
net netstat                              # Show active connections
net check-connection                     # Verify network connectivity
net bandwidth-test                       # Test download speed

# File Transfer
net download url localfile               # Download file from URL
net http-get url                         # Perform HTTP GET request

# Network Information
net mac                                  # Display MAC addresses
net route                               # Show routing information

# UDP Operations
net send-packet host port message        # Send UDP packet
```

## 🎯 Recent Updates

### Version 4.2.yz.24 (Current)
- 🆕 Introduced the experimental USH shell
- 🔄 Enhanced HQSH shell capabilities
- ✨ Added comprehensive macro system
- 🛠️ Expanded core command set

### Version 2.0.0 (2024-05-11)
- 🚀 Launched HQSH interface
- 📝 Added custom assembly language
- 💫 Introduced type-zza styling
- 💰 Added personal finance tracking

### Version 1.4.9 (2024-05-11)
- 🎮 Enhanced system resource management
- ⚡ Improved CTRL+C handling
- 🔧 Configuration file support
- 🎨 Enhanced customization options

## 🔮 Future Roadmap

We're actively developing exciting new features:

- 🌐 Enhanced f-Net protocol
- 🎮 Gaming optimizations
- 🔐 Advanced security features
- ☁️ Cloud integration
- 🤖 IoT device support

## 🤝 Contributing

We welcome contributions from the community! Whether you're fixing bugs, adding features, or improving documentation, your help makes Nova-S6 better for everyone.

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.