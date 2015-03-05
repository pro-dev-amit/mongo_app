:: Since, I've set things up like this in my local box. You just need to replace the paths here to point correctly. 

cd\
start /d "C:\redis_server" redis-server.exe
start /d "C:\mongodb_server\bin" mongod.exe --storageEngine wiredTiger --dbpath c:\data\db
start /d "C:\elasticsearch_server\bin" elasticsearch