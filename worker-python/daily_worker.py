import time
import schedule
import requests
from core.config import TELEGRAM_TOKEN, TELEGRAM_CHAT_ID
from services.technical_ai import analisa_teknikal
from services.sentiment_ai import analisa_sentimen
from services.backend_api import simpan_riwayat_ke_db

def kirim_notifikasi_telegram(pesan):
    if not TELEGRAM_CHAT_ID:
        print("[-] TELEGRAM_CHAT_ID kosong. Lewati pengiriman Telegram.")
        return
        
    url = f"https://api.telegram.org/bot{TELEGRAM_TOKEN}/sendMessage"
    payload = {"chat_id": TELEGRAM_CHAT_ID, "text": pesan, "parse_mode": "Markdown"}
    try:
        response = requests.post(url, json=payload)
        if response.status_code == 200:
            print("[+] Laporan harian berhasil dikirim ke Telegram.")
        else:
            print(f"[-] Gagal kirim ke Telegram: {response.text}")
    except Exception as e:
        print(f"[-] Error jaringan saat kirim Telegram: {e}")

def jalankan_analisa_harian():
    print("\n=== MEMULAI ANALISA HARIAN OTOMATIS (16:30) ===")
    
    daftar_saham = ["BBCA.JK", "BBNI.JK", "BBRI.JK", "BMRI.JK"]
    teks_laporan = "🤖 *LAPORAN HARIAN AI TRADER* 🤖\n"
    teks_laporan += "Menggabungkan Teknikal & Sentimen Berita\n\n"

    for kode in daftar_saham:
        print(f"-> Menganalisa {kode}...")
        
        # 1. Analisa Teknikal (Dari file services)
        status_tek, yakin_tek, harga, saham_obj = analisa_teknikal(kode)
        if status_tek is None:
            print(f"   [-] Data {kode} gagal ditarik.")
            continue
            
        # 2. Analisa Sentimen (Dari file services)
        status_sen, alasan_sen = analisa_sentimen(saham_obj, kode)

        # 3. Susun Teks Laporan
        teks_laporan += f"🏢 *{kode}* (Rp {harga:,.0f})\n"
        teks_laporan += f"📊 *Teknikal:* {status_tek} ({yakin_tek:.1f}%)\n"
        teks_laporan += f"📰 *Sentimen:* {status_sen}\n"
        teks_laporan += f"💬 _{alasan_sen}_\n"
        teks_laporan += "-------------------------\n"

        # 4. Push ke Database Backend .NET
        simpan_riwayat_ke_db(
            ticker=kode,
            teknikal=status_tek,
            probabilitas=yakin_tek,
            sentimen=f"{status_sen} - {alasan_sen}"
        )
        time.sleep(2) # Jeda agar tidak terkena limit API

    # 5. Kirim Laporan Lengkap ke Telegram
    kirim_notifikasi_telegram(teks_laporan)
    print("=== PROSES HARIAN SELESAI ===\n")

if __name__ == "__main__":
    print("[*] Daily Worker Berjalan. Menunggu jadwal pukul 16:30...")
    
    jam_eksekusi = "16:30"
    
    # Jadwalkan hanya di hari bursa buka (Senin - Jumat)
    schedule.every().monday.at(jam_eksekusi).do(jalankan_analisa_harian)
    schedule.every().tuesday.at(jam_eksekusi).do(jalankan_analisa_harian)
    schedule.every().wednesday.at(jam_eksekusi).do(jalankan_analisa_harian)
    schedule.every().thursday.at(jam_eksekusi).do(jalankan_analisa_harian)
    schedule.every().friday.at(jam_eksekusi).do(jalankan_analisa_harian)

    # UNCOMMENT baris di bawah ini jika Anda ingin langsung TES sekarang 
    # tanpa harus menunggu jam 16:30:
    # jalankan_analisa_harian()

    # Loop abadi untuk mengecek waktu
    while True:
        schedule.run_pending()
        time.sleep(60) # Cek setiap 1 menit agar CPU tidak terbebani