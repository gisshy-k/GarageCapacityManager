# Garage Capacity Manager

High-rise office buildings and high-density residential areas in Cities: Skylines 2 often have unrealistically small parking capacities, leading to excessive street parking.

**Garage Capacity Manager** solves this by dynamically scaling garage capacities based on the number of "households" for residential buildings and "workers" for commercial/office buildings. This scaling applies exclusively to invisible internal garages inside buildings.

I created this mod to be used alongside my other mod, *Realistic Time And Traffic (RTT)*, to achieve a more realistic cityscape and traffic simulation. It is also designed with compatibility in mind for mods like *MapExt* or *EconomyEX*.

## 🌟 Features
* **Residential Scaling:** Expands garage capacity based on the building's designed number of households. The default value is 0.5 (50% of households).
* **Workplace Scaling:** Dynamically calculates garage capacity based on the actual number of "workers" from companies currently renting the office or commercial building. The default value is 0.3 (30% of workers).
* **Mixed-Use Support:** For buildings with both residential and commercial spaces, the mod seamlessly combines both capacities (Households + Workers).
* **Full Customization:** You can easily adjust the parking allocation multipliers (0 to 1.0 = 0% to 100%) per household/worker at any time via the options menu sliders.

## ⚠️ IMPORTANT: Timing Constraints for Capacity Scaling
To completely eliminate any negative impact on simulation performance (CPU load), **there are intentional constraints on when scaling is applied**. In all cases below, **the new capacities will be applied the next time you load your save game, or whenever you adjust the sliders in the options menu**.
* **New Commercial/Offices:** Immediately after a commercial or office building is constructed, it may temporarily be assigned the vanilla initial parking capacity.
* **Building Level-Ups:** When a building levels up, the capacity changes will not be reflected instantly.

## 🛑 CRITICAL: Safe Uninstallation Instructions (Save Data Protection)
This mod strictly adheres to the "Vanilla Fallback Principle" to prevent save data corruption. However, due to how the game engine saves data, **you MUST follow these steps if you wish to uninstall (unsubscribe from) the mod:**
1. Launch the game and go to Options > Garage Capacity Manager.
2. Toggle **"Enable Garage Capacities"** to **OFF**.
3. Unpause the game and let the simulation run for a few seconds. The mod will safely restore all garage capacities in your city back to their original vanilla state.
4. **Save your game**.
5. You may now safely unsubscribe/uninstall the mod.

*Note: If you unsubscribe and load your game without following these steps, your game will still run fine, but existing buildings will permanently retain their modified (massive) garage capacities (until they level up or are rebuilt, at which point they will return to vanilla values).*

## 🤝 Compatibility & Safety
* **Safe to add/remove mid-game:** Yes, completely safe (provided you follow the steps above).
* **Performance Impact:** Minimal to none. We have entirely eliminated unnecessary background tracking processes.
* **Mod Compatibility:** Because this mod targets internal building parameters, it is **incompatible** with mods that affect roadside parking spaces (e.g., *Realistic Parking*). However, it is fully **compatible** with mods that do not target internal building parameters (e.g., *Remove Abandoned Cars*).

## ⚖️ Disclaimer
I am not a professional software engineer. This mod was built through dialogue with an AI assistant. The implementation methods have been carefully considered and thoroughly tested, and I am confident it runs stably, but I cannot guarantee perfect operation in all environments. Please use it at your own risk.

---
# Garage Capacity Manager

Cities: Skylines 2 の高層オフィスビルや高密度住宅は、駐車場のキャパシティが非現実的に小さく、街中に路上駐車があふれる原因になっています。

**Garage Capacity Manager** は、建物のガレージ容量を、住宅の「世帯数」および、商業・オフィスビルの「従業員数」に基づいて動的にスケールさせます。プレイヤーから見ることができない、建物内部に設置されたガレージのみがスケールの対象となります。

本MODは、私が作成した『Realistic Time And Traffic (RTT)』と併用し、よりリアルな都市の光景や交通シミュレーションを実現するために開発しました。また、『MapExt』や『EconomyEX』等のMODとの互換性も重視しています。

## 🌟 主な機能 (Features)
* **住宅のスケール:** 建物に設定されている設計上の世帯数に基づいてガレージ容量を拡張します。デフォルト値は0.5（世帯数の50％）です。
* **職場のスケール:** オフィスや商業ビルに実際に入居している企業の「従業員数」に基づいてガレージ容量を動的に計算します。デフォルト値は0.3（従業員数の30％）です。
* **複合施設への対応:** 住宅と商業スペースが混在する建物では、両方のキャパシティ（世帯数＋従業員数）を合算して適用します。
* **自由なカスタマイズ:** オプション画面のスライダーから、1世帯あたり／1従業員あたりの駐車枠の割り当て倍率（0〜1.0 = 0％〜100％）をいつでも調整可能です。

## ⚠️ 重要：スケールの適用タイミングの制約
シミュレーションのパフォーマンス（CPU負荷）への影響を極力排除するため、**スケール適用のタイミングに以下の一部制約**があります。いずれも**次回セーブデータをロードした際、またはオプション画面でスライダーを変更した際に自動的に適用されます**。
* **新規の商業・オフィス:** オフィスや商業ビルが建設された直後は、一時的にバニラの初期容量が割り当てられることがあります。
* **建物のレベルアップ:** 建物がレベルアップした場合も、即座には容量の変更が反映されません。

## 🛑 警告：安全なアンインストール手順（セーブデータの保護）
このMODは、プレイヤーのセーブデータ破損を防ぐための「バニラフォールバックの原則」を厳格に遵守して設計されています。しかし、ゲームエンジンのセーブ仕様上、**MODをアンインストール（サブスクライブ解除）する際は、必ず以下の手順を踏む必要があります。**
1. ゲームを起動し、オプション ＞ Garage Capacity Manager を開きます。
2. **「ガレージ容量の変更を有効にする（Enable Garage Capacities）」を OFF** にします。
3. ゲームの時間を進め（ポーズを解除し）、数秒間シミュレーションを動かします。これにより、MODが都市内のすべてのガレージ容量を本来のバニラ状態へ安全に復元します。
4. **ゲームをセーブします。**
5. これで、安全にMODをサブスクライブ解除（アンインストール）できます。

*※注意：この手順を踏まずにいきなりMODを解除してロードした場合でも、ゲームは問題なく動作しますが、既存の建物にはMODで拡張された巨大なガレージ容量がそのまま半永久的に残ります（建物がレベルアップするか、建て替えられるとバニラ値に戻ります）。*

## 🤝 互換性と安全性
* **途中導入・途中削除:** 完全に安全です（上記の手順を守った場合）。
* **パフォーマンスへの影響:** バックグラウンドでの不要な常時監視プロセスを完全に排除しているため、極小です。
* **他のMODとの互換性:** 建物の内部パラメータのみをターゲットにしているため、道路脇の駐車スペースに影響を与えるMOD（例：*Realistic Parking* など）との併用はできません。一方で、建物の内部パラメータをターゲットにしていないMOD（例：*Remove Abandoned Cars* など）とは併用が可能です。

## ⚖️ 免責事項
私は本職のエンジニアではありません。このMODはAIをアシスタントとした対話によって構築されました。実装方法は詳細に検討し、動作テストも行っており安定的に動くと確信していますが、動作を完全に保証するものではありません。自己責任でのご利用をお願いいたします。
