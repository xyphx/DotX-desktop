FROM golang:1.22-alpine AS builder
WORKDIR /src
COPY server/go.mod server/go.sum ./
RUN go mod download
COPY server/. .
RUN go build -o /bin/server ./cmd

FROM alpine:3.20
COPY --from=builder /bin/server /bin/server
EXPOSE 8080
CMD ["/bin/server"]
