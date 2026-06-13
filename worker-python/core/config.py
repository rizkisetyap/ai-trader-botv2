import os
from dotenv import load_dotenv

load_dotenv()

TELEGRAM_TOKEN = os.getenv("TELEGRAM_TOKEN")
TELEGRAM_CHAT_ID = os.getenv("TELEGRAM_CHAT_ID") # Tambahkan ini
GEMINI_API_KEY = os.getenv("GEMINI_API_KEY")

# URL API .NET Anda. Ganti IP jika di VPS. Jika lokal Docker, gunakan nama service (misal: http://api:8080/...)
# Untuk tes lokal di laptop Anda saat ini:
BACKEND_API_URL = os.getenv("BACKEND_API_URL", "http://localhost:5000/api/v1/Saham/terima-analisa")

if not all([TELEGRAM_TOKEN, GEMINI_API_KEY]):
    raise ValueError("Kredensial API tidak lengkap! Cek file .env Anda.")