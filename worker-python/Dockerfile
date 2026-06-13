FROM python:3.12-slim

WORKDIR /app

# Instal dependensi sistem yang diperlukan untuk kompilasi scikit-learn & pandas
RUN apt-get update && apt-get install -y --no-install-recommends \
    build-essential \
    && rm -rf /var/lib/apt/lists/*

# Copy requirements dan instal pustaka Python
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy seluruh kode program modular ke dalam kontainer
COPY . .

# Secara default menjalankan bot interaktif, 
# perintah ini akan kita override di docker-compose untuk scheduler
CMD ["python", "main.py"]