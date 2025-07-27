import torch, io, sys, os, logging, warnings
from flask import Flask, request, Response, render_template
from langdetect import detect
from TTS.api import TTS

# warnings.simplefilter(action='ignore', category=FutureWarning)
# sys.stdout = open(os.devnull, 'w')
# logging.disable(logging.CRITICAL)

print(f"Python executable(for gebug): {sys.executable}")

device = "cuda" if torch.cuda.is_available() else "cpu"
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
MODEL_PATH = os.path.join(BASE_DIR, "TTSModel")
CONFIG_PATH = os.path.join(BASE_DIR, "TTSModel", "config.json")
tts = TTS(model_path=MODEL_PATH, config_path=CONFIG_PATH, progress_bar=False)
tts.to(device)

VOICES = {
    'en': "./Voices/en_voice.wav",
    'es': "./Voices/es_voice.wav",
    'fr': "./Voices/fr_voice.wav",
    'de': "./Voices/de_voice.wav",
    'it': "./Voices/it_voice.wav",
    'pt': "./Voices/pt_voice.wav",
    'pl': "./Voices/pl_voice.wav",
    'tr': "./Voices/tr_voice.wav",
    'ru': "./Voices/ru_voice.wav",
    'nl': "./Voices/nl_voice.wav",
    'cs': "./Voices/cs_voice.wav",
    'ar': "./Voices/ar_voice.wav",
    'zh-cn': "./Voices/zh_cn_voice.wav",
    'hu': "./Voices/hu_voice.wav",
    'ko': "./Voices/ko_voice.wav",
    'ja': "./Voices/ja_voice.wav",
    'hi': "./Voices/hi_voice.wav"
}

app = Flask(__name__)


@app.route('/')
def index():
    return render_template("index.html")

@app.route('/tts', methods=['POST'])
def speak():
    text = request.data.decode('utf-8')
    try:
        lang = detect(text)
    except:
        lang = "en"

    # Default to English if language not supported
    speaker_file = VOICES.get(lang, VOICES['en'])

    buf = io.BytesIO()
    with torch.inference_mode():
        tts.tts_to_file(
            text=text,
            speaker_wav=speaker_file,
            language=lang,
            file_path=buf
        )
    buf.seek(0)
    data = buf.read()
    buf.close()

    return Response(data, mimetype="audio/wav")


if __name__ == "__main__":
    app.run(port=7777, threaded=True)