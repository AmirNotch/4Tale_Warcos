# Как обновлять компоненты на сервере

## Залогиниться на сервере
- [ ] В командной строке ввести ssh k8s@5.39.218.77
- [ ] Ввести пароль (который можно получить у менеджера проекта)
- [ ] Перейти в директорию с kubernetes-конфигами: cd kube-configs-warcos/kubernetes-configs

## Работа с kubernetes-кластером на сервере
- [ ] Посмотреть список всех подов в кластере: kubectl get pods -A
- [ ] Посмотреть список файлов в директории: ls -la
- [ ] Открыть конфиг на редактирование (на примере Lobby): vim lobby.yml
- [ ] Применить конфиг после редактирования: kubectl apply -f lobby.yml

# Другие полезные команды при работе с kubernetes-кластером
- [ ] Получить список gameservers (Agones): kubectl get gs
- [ ] Удалить gameserver: kubectl delete gs warcos-game-server-legit
- [ ] Загрузить логи из kubernetes-pod'а: kubectl logs --tail=200 -n warcos-mmf warcos-matchfunction-698bb9dcfd-jlr2z | grep something
- [ ] [Инструкция: как использовать в kubernetes docker-образы из корпоративного Gitlab](https://docs.google.com/document/d/1FnPk03P_ySTYa8IUZuYEcK5vu0OxbSCw97WSHdnAeWw/edit?tab=t.0)
