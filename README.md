# serilog
Aprendendo Melhor como usar o Serilog

usei como referencia o excelenta artigo [https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-logging-the-selected-endpoint-name-with-serilog/]


docker run \
  --name seq \
  -d \
  --restart unless-stopped \
  -e ACCEPT_EULA=Y \
  -v /home/fernandocardo/seq:/data \
  -p 8080:80 \
  -p 5341:5341 \
  datalust/seq:latest