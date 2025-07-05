# ❓ FAQ

---

## ⚙️ General

---

### Why use Coqui XTTS as the TTS model?

Three main reasons:

- **Custom Voices**.

    You can use any voice you want: your own, Cyn, Vito Corleone, Justin Timberlake — whatever.
I believe that’s way more flexible than static, pre-generated voices.

- **License**.

    Some TTS models have extremely restrictive licenses. For example, Silero TTS is under CC BY-NC-SA 4.0, which means:
    - You can’t use it commercially.
    - Even worse: I’d be forced to release this entire framework under the same license.
That’s a no-go.
  
- **Simplicity**.

    The Python server is **just ~100 lines of code**.
It supports 10+ languages out of the box.
You send a UnityWebRequest with text — it sends back .wav.
That’s it. No ONNX, no weird tensors, no converting text into token IDs, etc.


But if you hate Python or Coqui XTTS, you can integrate your favorite model.
Modularity is the point. I won’t stop you 🤠

---

## 🐍 Python

---

### Why use Python for TTS?

I didn’t really have a choice.

**Coqui XTTS only offers a Python API**, and there are no stable C#, C++, C, TypeScript, JavaScript or Go bindings.
If there had been a solid alternative in another language, I would’ve gladly used it — but this is what we’ve got.

---

### Why is Python.exe using so much RAM?
Because it’s running a full neural TTS model — locally — in Python.
That’s the price you pay for realistic voices and multilingual support.
If it bothers you — try switching to smaller models or optimize the server logic yourself.

---

## ❗ Most Important Question

---

### Why is it so hard to configure and get started?

A totally fair question — especially after you’ve gone through all the setup steps and seen how many pieces are involved.

Here’s the truth:
> UnityNeuroSpeech is the first — not just Unity, but game development framework — that allows you to talk to AI directly inside your game.

---

The only out-of-the-box solution for Unity in this whole stack is [whisper.unity](https://github.com/Macoron/whisper.unity). 
And even then — you still need to create a separate `SetupWhisperPath.cs` script to make it work properly in builds. Yes, you can try using **StreamingAssets**, and whisper.unity supports that.
But I personally prefer not to scatter files across multiple Unity folders.
If that’s critical for your project — feel free to try it with StreamingAssets.

---

Now let’s talk about Text-to-Speech...

Frankly, there are no really “clean” or “easy” TTS solutions — not just for Unity, **but even for C# as a whole**.
(If you’re curious about the deep rabbit hole of TTS licensing and tech, check out the [Python Server](server.md) page.)

---

As for **Ollama**, I integrated it into Unity myself — using Microsoft’s `Microsoft.Extensions` libraries.

---

Yes — it does feel like a lot at first.

But don’t worry — **UnityNeuroSpeech is actively being improved**, and with every update, the setup process will become simpler and more automated.

---

And maybe one day, this kind of tech will be built into Unity out of the box.
You’ll click a button, and boom — talking AI agents everywhere.

But when that day comes... **it won’t be unique anymore.**

So if you want to stand out with a game that features **real, emotional voice interaction powered by AI** — now’s your chance.
Go for it 😎