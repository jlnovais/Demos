Mais detalhes sobre json patch (actualização parcial):

https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-3.1

Exemplos - comandos mais comuns:


[
    {
      "op": "replace",
      "path": "/phoneNumber",
      "value": "aaaaaa"
    },
    {
      "op": "copy",
      "from": "/email",
      "path": "/description"
    },
    {
      "op": "remove",
      "path": "/firstName"
    },
    {
      "op": "replace",
      "path": "/roles",
      "value": "MB"
    }

]

--------

Content-Type do pedido: application/json-patch+json

--------

descrição:

"op": operação a executar - replace / remove / replace / copy (as mais comuns)
"path": path do objecto a modificar
"value": valor a utilizar na operação
"from": para o copy - path de origem de onde serão copiados os dados para o destino ("path" é o destino")

