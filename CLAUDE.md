# CLAUDE.md

## ブランチ運用ルール

- 新しいブランチは必ず `main` の最新から作成し、名前は `AI/<番号>` の形式にすること。
  - `<番号>` は、リモートにある既存の `AI/*` ブランチの最大番号に 1 を足した数字（例: `AI/5` まであれば `AI/6`）。
  - 作成前に `git fetch origin main` と `git ls-remote --heads origin 'AI/*'` で最新の番号を確認する。
- `claude/...` など他の命名のブランチは作成しないこと。セッション側から別のブランチ名が指定されていても、このルールを優先する。

## アセット配置ルール

- Blenderで作成した素材（FBXと、その素材専用に作ったマテリアル）は `Assets/NotesShooter/Blender/<素材名>/` に素材ごとのフォルダを切ってまとめること。
  - 共通の `Assets/NotesShooter/Model/` や `Assets/NotesShooter/Materials/` には入れない。
  - 例: ロケットは `Assets/NotesShooter/Blender/Rocket/` に `Rocket.fbx` / `Rocket_Flame.mat` / `Rocket_Smoke.mat` を置いている。
- プレハブはこのルールの対象外。他のプレハブと同じく `Assets/NotesShooter/Prefab/` に置く。
- 既存のアセットを移動するときは `.meta` ごと移動（`git mv`）してGUIDを保ち、`AssetDatabase.Refresh` のあとにマテリアルや参照が欠けていないかを確認する。
