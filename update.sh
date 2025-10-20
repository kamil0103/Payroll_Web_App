#!/bin/bash

# Navigate to your project
cd /mnt/user/appdata/Payroll_Web_App

# Pull latest changes from GitHub
git reset --hard
git pull origin master

# Rebuild and restart Docker containers
docker compose down
docker compose up -d --build
