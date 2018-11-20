FROM mono:5.12
RUN mkdir -p /ns
WORKDIR /ns
COPY . /ns
EXPOSE 33033
CMD ["mono", "./bin/Debug/NamingServer.exe"]
