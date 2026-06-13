import telebot
from core.config import TELEGRAM_TOKEN
from services.technical_ai import analisa_teknikal
from services.sentiment_ai import analisa_sentimen
from services.backend_api import simpan_riwayat_ke_db

bot = telebot.TeleBot(TELEGRAM_TOKEN)

@bot.message_handler(commands=['start', 'help'])
def send_welcome(message):
    bot.reply_to(message, "🤖 *AI Trader Bot Ready!*\nKetik `/analisa KODE_SAHAM` (contoh: `/analisa BBCA.JK`)", parse_mode="Markdown")

@bot.message_handler(commands=['analisa'])
def handle_analisa(message):
    try:
        # Ekstrak Ticker dari pesan (misal: /analisa BBCA.JK)
        kode_saham = message.text.split()[1].upper()
    except IndexError:
        bot.reply_to(message, "⚠️ Format salah! Gunakan: `/analisa KODE_SAHAM`", parse_mode="Markdown")
        return

    msg = bot.reply_to(message, f"⏳ Menganalisa *{kode_saham}*... (Membaca Teknikal & Sentimen)", parse_mode="Markdown")

    # 1. Otak Kiri (Teknikal)
    status_tek, yakin_tek, harga, saham_obj = analisa_teknikal(kode_saham)
    if status_tek is None:
        bot.edit_message_text(f"❌ Data saham {kode_saham} tidak ditemukan.", chat_id=message.chat.id, message_id=msg.message_id)
        return

    # 2. Otak Kanan (Sentimen)
    status_sen, alasan_sen = analisa_sentimen(saham_obj, kode_saham)

    # 3. Susun Laporan untuk User
    laporan = (
        f"🏢 *{kode_saham}* (Rp {harga:,.0f})\n\n"
        f"📊 *Teknikal:* {status_tek} ({yakin_tek:.1f}%)\n"
        f"📰 *Sentimen:* {status_sen}\n"
        f"💬 _{alasan_sen}_"
    )
    bot.edit_message_text(laporan, chat_id=message.chat.id, message_id=msg.message_id, parse_mode="Markdown")

if __name__ == "__main__":
    print("[*] Worker Python (Telegram Bot) berjalan...")
    bot.infinity_polling()