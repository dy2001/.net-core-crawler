# .net-core-crawler
.net core 3.0+PuppeteerSharp+AngleSharp写的腾讯漫画爬虫

在执行第一行的时候会判断你的项目文件夹是否有chromium浏览器，如果没有会自动下载，不过这个在国内的网络是下不了的(原因你懂得)。
或者你在程序的debug目录`你的程序.exe\.local-chromium\Win64-706915\chrome-win\`想这样建一个文件夹，再把自己从别的地方下载的chromium浏览器放在chrome-win里也是可以的。