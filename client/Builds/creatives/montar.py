"""Criativos 9:16 do Rune Relay (docs/CRIATIVOS.md).
  python client/Builds/creatives/montar.py gravar   -> grava os clipes no build (janela visivel!) e monta os videos
  python client/Builds/creatives/montar.py [rr_01 ...] -> so monta, a partir dos quadros ja gravados
Quadros e textos ficam em %TEMP%/rr_rec (ou RR_REC); os MP4 e as folhas de contato, ao lado deste arquivo."""
import glob, os, subprocess, sys, tempfile

OUT = os.path.dirname(os.path.abspath(__file__))
EXE = os.path.join(OUT, "..", "win", "RuneRelay.exe")
REC = os.environ.get("RR_REC", os.path.join(tempfile.gettempdir(), "rr_rec"))
FPS, SKIP = 30, 2                      # os 2 primeiros quadros sao de antes do roteiro -demo agir
IMPACT = r"C\:/Windows/Fonts/impact.ttf"
GOLD = "0xFFD66B"                      # Art.Accent

CLIPS = {                              # clipe: (nivel, roteiro -demo)
    "c5a": (29, "solve:0 wait:0.5 release wait:1.0"),
    "c5b": (26, "solve:0 wait:0.5 release wait:1.0"),
    "c5c": (25, "solve:0 wait:0.5 release wait:1.0"),
    "c1": (30, "almost wait:0.6 release solve:0.6 wait:0.5 release wait:1.0"),
    "c7a": (1, "solve:0.25 wait:0.3 release wait:0.8"),
    "c7b": (10, "solve:0.18 wait:0.3 release wait:0.8"),
    "c7c": (30, "solve:0.15 wait:0.3 release wait:1.0"),
    "c10": (28, "almost:0.2 wait:0.4 release wait:2.5"),
}


def gravar(name):
    level, demo = CLIPS[name]
    d = os.path.join(REC, name)
    subprocess.run([EXE, "-screen-fullscreen", "0", "-screen-width", "540", "-screen-height", "960", "-level", str(level),
                    "-demo", demo, "-record", d, "-logFile", d + ".log"], check=True, timeout=600)
    print(name, len(glob.glob(f"{d}/f*.png")), "quadros")


def dt(name, s, size, y, t0=None, t1=None, color="white", fade=False):
    with open(os.path.join(REC, f"txt_{name}.txt"), "w", encoding="utf-8", newline="\n") as f:  # CRLF vira linha dupla
        f.write(s)
    en = f":enable='between(t,{t0},{t1})'" if t0 is not None else ""
    al = ":alpha='min(1,t/0.25)'" if fade else ""
    return (f"drawtext=fontfile='{IMPACT}':textfile='txt_{name}.txt':fontsize={size}:fontcolor={color}"
            f":borderw={max(6, size // 13)}:bordercolor=black:text_align=C:line_spacing=-8"
            f":x=(w-text_w)/2:y={y}{en}{al}")


def build(out, clips, end_sub=None, end_sec=1.5):
    args, fc, n = ["ffmpeg", "-y", "-v", "error"], [], 0
    for i, (name, vf) in enumerate(clips):
        frames = sorted(glob.glob(f"{REC}/{name}/f*.png"))
        dur = (len(frames) - SKIP) / FPS
        audio = os.path.basename(glob.glob(f"{REC}/{name}/audio_*.f32")[0])
        rate, ch = audio[6:-4].split("_")
        args += ["-framerate", str(FPS), "-start_number", str(SKIP), "-i", f"{name}/f%05d.png",
                 "-ss", f"{SKIP / FPS:.4f}", "-f", "f32le", "-ar", rate, "-ac", ch, "-i", f"{name}/{audio}"]
        fc.append(f"[{n}:v]{','.join(vf + ['format=yuv420p', 'setsar=1'])}[v{i}]")
        fc.append(f"[{n + 1}:a]aresample=48000,aformat=channel_layouts=stereo,apad,atrim=0:{dur:.4f}[a{i}]")
        n += 2
    # cartao final: ultimo quadro desfocado e escurecido + titulo
    args += ["-loop", "1", "-framerate", str(FPS), "-t", str(end_sec), "-i", os.path.relpath(frames[-1], REC),
             "-f", "lavfi", "-t", str(end_sec), "-i", "anullsrc=r=48000:cl=stereo"]
    end = ["gblur=sigma=26", "eq=brightness=-0.25:saturation=0.7",
           dt("titulo", "RUNE RELAY", 190, "(h-text_h)/2-70", color=GOLD, fade=True)]
    if end_sub:
        end.append(dt(out + "_cta", end_sub, 84, "h/2+80", fade=True))
    k = len(clips)
    fc.append(f"[{n}:v]{','.join(end + ['format=yuv420p', 'setsar=1'])}[v{k}]")
    fc.append(f"[{n + 1}:a]anull[a{k}]")
    fc.append("".join(f"[v{i}][a{i}]" for i in range(k + 1)) + f"concat=n={k + 1}:v=1:a=1[vc][ac]")
    fc.append("[ac]volume=6dB,alimiter=limit=0.9[ao]")
    mp4 = os.path.join(OUT, out + ".mp4")
    args += ["-filter_complex", ";".join(fc), "-map", "[vc]", "-map", "[ao]", "-c:v", "libx264", "-preset", "medium",
             "-crf", "18", "-pix_fmt", "yuv420p", "-r", str(FPS), "-c:a", "aac", "-b:a", "160k", "-movflags", "+faststart", mp4]
    subprocess.run(args, cwd=REC, check=True)
    subprocess.run(["ffmpeg", "-y", "-v", "error", "-i", mp4, "-vf", "fps=1/2,scale=270:-1,tile=6x2", "-frames:v", "1",
                    os.path.join(OUT, out[:5] + "_contato.png")], check=True)
    print(out, "ok")


HOOK_Y = "330-text_h/2"   # topo do tabuleiro: abaixo da UI do TikTok/Reels, longe do cartao de falha
VIDEOS = {
    "rr_05": lambda: build("rr_05_cascata", [("c5a", []), ("c5b", []), ("c5c", [])]),
    "rr_01": lambda: build("rr_01_gire_uma_peca", [("c1", [dt("h1", "SÓ GIRE\nUMA PEÇA", 140, HOOK_Y, 0, 4.0)])]),
    "rr_07": lambda: build("rr_07_facil_dificil", [
        ("c7a", [dt("h7a", "NÍVEL 1\nFÁCIL", 140, HOOK_Y, 0, 2.2)]),
        ("c7b", [dt("h7b", "NÍVEL 10\nHMM...", 140, HOOK_Y, 0, 1.8)]),
        ("c7c", [dt("h7c", "NÍVEL 30\nE AGORA?", 140, HOOK_Y, 0, 2.0)])]),
    "rr_10": lambda: build("rr_10_voce_faria_melhor", [("c10", [
        dt("h10", "ACHA\nFÁCIL?", 140, HOOK_Y, 0, 2.5),
        dt("c10", "VOCÊ FARIA\nMELHOR?", 150, "h*0.42-text_h/2", 5.3, 99)])],   # 5.3 s: logo depois do "Cor errada!"
        end_sub="VOCÊ FARIA MELHOR?", end_sec=2.0),
}

if __name__ == "__main__":
    os.makedirs(REC, exist_ok=True)
    ids = sys.argv[1:]
    if ids[:1] == ["gravar"]:
        for c in CLIPS:
            gravar(c)
        ids = ids[1:]
    for v in ids or VIDEOS:
        VIDEOS[v]()
