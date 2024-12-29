Network Monitor tool
====================

I’ve been having trouble with my internet lately. I have a 1 Gbps down/100 Mbps up connection, but my PC regularly dips down to 100/100 Mbps link speeds, vastly limiting the bandwith I actually get, happening seemingly at random. 

I’ve had ISP technicians look into limiting the signal strength, which would seem to help for a while. Restarting my PC doesn’t help, only physically unplugging the ethernet cable and plugging it back in again. 

In an effort to learn more, I made this tool that simply monitors the network interfaces and checks the status and link speed of the one you’re using. It looks something like this:

```
 [WARN] 2024-12-29T08:38:26 LowLinkSpeed
[DEBUG] 2024-12-29T09:39:26 No change in last hour.
[DEBUG] 2024-12-29T10:07:57 Network address changed. ⎫
[DEBUG] 2024-12-29T10:07:57 Network address changed. ⎬ (Unplugging the ethernet cable.)
[DEBUG] 2024-12-29T10:07:57 Network address changed. ⎭
[DEBUG] 2024-12-29T10:08:04 Network address changed. ⎫
[DEBUG] 2024-12-29T10:08:04 Network address changed. ⎬ (Plugging the ethernet cable back in.)
[DEBUG] 2024-12-29T10:08:04 Network address changed. ⎭
 [INFO] 2024-12-29T10:08:04 Normal
[DEBUG] 2024-12-29T10:08:06 Network address changed.
```
