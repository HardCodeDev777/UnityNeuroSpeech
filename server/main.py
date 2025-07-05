import torch, io, sys, os, logging, warnings
from flask import Flask, request, Response, render_template
from langdetect import detect
from torch.serialization import add_safe_globals
from TTS.api import TTS
from TTS.tts.configs.xtts_config import XttsConfig
from TTS.tts.models.xtts import XttsAudioConfig, XttsArgs
from TTS.config.shared_configs import BaseDatasetConfig
add_safe_globals([XttsConfig, XttsAudioConfig, XttsArgs, BaseDatasetConfig])

warnings.simplefilter(action='ignore', category=FutureWarning)
sys.stdout = open(os.devnull, 'w')
logging.disable(logging.CRITICAL)

device = "cuda" if torch.cuda.is_available() else "cpu"
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
MODEL_PATH = os.path.join(BASE_DIR, "TTSModel")
CONFIG_PATH = os.path.join(BASE_DIR, "TTSModel", "config.json")
tts = TTS(model_path=MODEL_PATH, config_path=CONFIG_PATH, progress_bar=False)
tts.to(device)

EN_VOICE = "./Voices/en_voice.wav"
ES_VOICE = "./Voices/es_voice.wav"
FR_VOICE = "./Voices/fr_voice.wav"
DE_VOICE = "./Voices/de_voice.wav"
IT_VOICE = "./Voices/it_voice.wav"
PT_VOICE = "./Voices/pt_voice.wav"
PL_VOICE = "./Voices/pl_voice.wav"
TR_VOICE = "./Voices/tr_voice.wav"
RU_VOICE = "./Voices/ru_voice.wav"
NL_VOICE = "./Voices/nl_voice.wav"
CS_VOICE = "./Voices/cs_voice.wav"
AR_VOICE = "./Voices/ar_voice.wav"
ZH_CN_VOICE = "./Voices/zh_cn_voice.wav"
HU_VOICE = "./Voices/hu_voice.wav"
KO_VOICE = "./Voices/ko_voice.wav"
JA_VOICE = "./Voices/ja_voice.wav"
HI_VOICE = "./Voices/hi_voice.wav"
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
    if lang not in ['en', 'es', 'fr', 'de', 'it', 'pt', 'pl', 'tr', 'ru', 'nl', 'cs', 'ar', 'zh-cn', 'hu', 'ko', 'ja', 'hi']:
        lang = "en"

    if(lang == "en"):
        speaker_file = EN_VOICE
    elif(lang == "es"):
        speaker_file = ES_VOICE
    elif(lang == "fr"):
        speaker_file = FR_VOICE
    elif(lang == "de"):
        speaker_file = DE_VOICE
    elif(lang == "it"):
        speaker_file = IT_VOICE
    elif(lang == "pt"):
        speaker_file = PT_VOICE
    elif(lang == "pl"):
        speaker_file = PL_VOICE
    elif(lang == "tr"):
        speaker_file = TR_VOICE
    elif(lang == "ru"):
        speaker_file = RU_VOICE
    elif(lang == "nl"):
        speaker_file = NL_VOICE
    elif(lang == "cs"):
        speaker_file = CS_VOICE
    elif(lang == "ar"):
        speaker_file = AR_VOICE
    elif(lang == "zh-cn"):
        speaker_file = ZH_CN_VOICE
    elif(lang == "hu"):
        speaker_file = HU_VOICE
    elif(lang == "ko"):
        speaker_file = KO_VOICE
    elif(lang == "ja"):
        speaker_file = JA_VOICE
    elif(lang == "hi"):
        speaker_file = HI_VOICE

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

if __name__=="__main__":
    app.run(port=7777, threaded=True)