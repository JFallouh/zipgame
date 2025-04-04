# Final image: Just copy the published output
FROM debian:bookworm-slim AS runtime
WORKDIR /app

# Add required dependencies manually (for .NET apps)
RUN apt-get update && apt-get install -y libicu-dev libssl-dev && rm -rf /var/lib/apt/lists/*

COPY out/ ./
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["./zipgame"]
