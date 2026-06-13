import os
import joblib
import numpy as np
import pandas as pd
import yfinance as yf
import pandas_ta as ta
import requests  # <-- 1. Pastikan requests di-import
from sklearn.ensemble import RandomForestClassifier
from sklearn.model_selection import train_test_split

# --- 2. TAMBAHKAN BLOK TOPENG (USER-AGENT) INI ---
sesi_browser = requests.Session()
sesi_browser.headers.update({
    'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
})
# -------------------------------------------------
def analisa_teknikal(ticker_code):
    # --- 3. SUNTIKKAN SESI KE DALAM TICKER ---
    saham = yf.Ticker(ticker_code, session=sesi_browser)
    saham = yf.Ticker(ticker_code)
    df = saham.history(period="5y")
    
    if df.empty:
        return None, None, None, saham
        
    df['SMA_20'] = ta.sma(df['Close'], length=20)
    df['SMA_50'] = ta.sma(df['Close'], length=50)
    df['RSI_14'] = ta.rsi(df['Close'], length=14)
    macd = ta.macd(df['Close'])
    df['MACD'] = macd.iloc[:, 0]
    df['MACD_Signal'] = macd.iloc[:, 1]
    bbands = ta.bbands(df['Close'])
    df['BB_Lower'] = bbands.iloc[:, 0]
    df['BB_Upper'] = bbands.iloc[:, 2]

    df['Target'] = np.where(df['Close'].shift(-1) > df['Close'], 1, 0)
    data_hari_ini = df.iloc[-1:].copy()
    df = df.dropna()

    fitur = ['SMA_20', 'SMA_50', 'RSI_14', 'MACD', 'MACD_Signal', 'BB_Lower', 'BB_Upper', 'Open', 'Close', 'Volume']
    
    nama_file_memori = f"memori_ai_{ticker_code}.joblib"
    if os.path.exists(nama_file_memori):
        model = joblib.load(nama_file_memori)
    else:
        X = df[fitur]
        y = df['Target']
        X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, shuffle=False)
        model = RandomForestClassifier(n_estimators=200, max_depth=10, random_state=42)
        model.fit(X_train, y_train)
        joblib.dump(model, nama_file_memori)

    prediksi = model.predict(data_hari_ini[fitur])[0]
    prob = model.predict_proba(data_hari_ini[fitur])[0]
    
    status = "NAIK 🟢" if prediksi == 1 else "TURUN/DATAR 🔴"
    keyakinan = prob[1] * 100 if prediksi == 1 else prob[0] * 100
    harga = data_hari_ini['Close'].values[0]
    
    return status, keyakinan, harga, saham