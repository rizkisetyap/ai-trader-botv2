import google.generativeai as genai
from core.config import GEMINI_API_KEY

genai.configure(api_key=GEMINI_API_KEY)
model_ai_teks = genai.GenerativeModel('gemini-1.5-flash')

def analisa_sentimen(saham_ticker, nama_saham):
    try:
        berita = saham_ticker.news
        if not berita:
            return "NETRAL ⚪", "Tidak ada berita hari ini."

        # HAPUS BARIS INI:
        # kumpulan_judul = "\n".join([f"- {b['title']}" for b in berita[:3]])
        
        # GANTI DENGAN BLOK INI:
        kumpulan_judul_list = []
        for b in berita[:3]:
            # Ambil judul, jika tidak ada cari di dalam 'content', jika tidak ada juga beri teks default
            judul = b.get('title', '')
            if not judul and 'content' in b:
                judul = b['content'].get('title', 'Berita tanpa judul')
            
            kumpulan_judul_list.append(f"- {judul}")
            
        kumpulan_judul = "\n".join(kumpulan_judul_list)
        prompt = f"""
        Kamu analis saham profesional. Baca 3 judul berita saham {nama_saham} berikut:
        {kumpulan_judul}
        Apakah sentimennya Positif, Negatif, atau Netral untuk harga saham? 
        Jawab dengan format: [STATUS] | [Alasan singkat 1 kalimat]
        """
        
        respon = model_ai_teks.generate_content(prompt)
        teks = respon.text.strip()
        
        if "|" in teks:
            status, alasan = teks.split("|", 1)
            status = status.strip().upper()
            if "POSITIF" in status: status = "POSITIF 🟢"
            elif "NEGATIF" in status: status = "NEGATIF 🔴"
            else: status = "NETRAL ⚪"
            return status, alasan.strip()
        return "NETRAL ⚪", "AI gagal memformat jawaban."
    except Exception as e:
        return "NETRAL ⚪", f"Error baca sentimen: {e}"