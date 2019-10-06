FROM node:10.16-alpine
RUN apk add --no-cache git && \
    cd /home && \
    git clone https://github.com/s60912frank/BattleMaster.git && \
    mv BattleMaster/Server . && \
    rm -r BattleMaster && \
    cd Server && \
    npm install
CMD [ "node", "/home/Server/server.js" ]