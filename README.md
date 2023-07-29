# SocketTest
基于c#一个简单的socket通信服务器和客户端的实现，用来进行简单的服务器和客户端的消息收发

## 使用方法
使用visual studio打开，启动server和client两个项目  
![image](https://github.com/xuehao-in-studing/SocketTest/assets/102791379/f996615c-6bf8-4344-a25b-184adcca2f07)  
服务器搭载在本机上面，运行系统为windows。  
上方是服务器的简单界面，可以在文本框内设置本机服务器的IP和端口号，开启服务器之后就可以监听客户端的连接，客户端连接之后在接受框显示服务器IP地址、发送的字节数和发送消息。  

![image](https://github.com/xuehao-in-studing/SocketTest/assets/102791379/2fdc9e91-54c6-4e6b-88e0-5022bbbefcdb)
上方是客户端的简单界面，可以设置要连接的服务器的IP和端口号，连接服务器后在状态框会显示当前连接状态，可以通过接收端和发送端两个框跟服务器进行一个简单的收发。  

### 待解决的点
怎么解决服务器丢包问题，怎么处理多个服务器连接，多线程的管理问题。

