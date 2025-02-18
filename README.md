# Стек
- VContainer
- UniRx
- UniTask
- DoTween
# Смысл
Архитектура разделенная на состояния, где для каждого состояния существует свой контейнер.
Никаких игровых сцен не существует заранее, весь игровой мир создается при переходе в состояние игры из заранее заготовленных сущностей и правил. 
# Entry point 
BootstrapInstaller.cs -> BootstrapEntryPoint.cs.
Регистрация контейнеров происходит в наследуемых от ScriptableInstaller.cs скриптах (SO) для каждого скоупа

https://github.com/user-attachments/assets/ec63f36e-e76c-4278-9a18-fe97cb57afed

