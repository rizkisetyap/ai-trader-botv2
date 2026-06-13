import requests
from core.config import BACKEND_API_URL

def simpan_riwayat_ke_db(ticker, teknikal, probabilitas, sentimen):
    payload = {
        "ticker": ticker,
        "prediksiTeknikal": teknikal,
        "probabilitasKenaikan": float(probabilitas),
        "sentimen": sentimen
    }
    try:
        response = requests.post(BACKEND_API_URL, json=payload)
        if response.status_code in [200, 201, 202]:
            print(f"[+] Data {ticker} berhasil di-push ke backend .NET.")
        else:
            print(f"[-] Gagal push ke backend. Status: {response.status_code}")
    except Exception as e:
        print(f"[-] Error koneksi ke backend .NET: {e}")