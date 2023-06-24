// var express = require('express');
// var app = express();

// app.configure(function(){
// 	app.use(express.methodOverride());
// 	app.use(express.bodyParser());
// 	app.use(app.router);
// 	app.set('view engine', 'jade');
// 	app.set('views', __dirname + '/public');
// 	app.set('view options', {layout: false});
// 	app.set('basepath',__dirname + '/public');
// });

// app.configure('development', function(){
// 	app.use(express.static(__dirname + '/public'));
// 	app.use(express.errorHandler({ dumpExceptions: true, showStack: true }));
// });

// app.configure('production', function(){
// 	var oneYear = 31557600000;
// 	app.use(express.static(__dirname + '/public', { maxAge: oneYear }));
// 	app.use(express.errorHandler());
// });

// console.log("Web server has started.\nPlease log on http://127.0.0.1:3001/index.html");
// app.listen(3001);




var express = require('express');
var methodOverride = require('method-override');
var app = express();

// 设置中间件
app.use(methodOverride('_method'));
app.use(express.json()); // 替换 bodyParser() 方法
app.use(express.urlencoded({ extended: true })); // 替换 bodyParser() 方法
app.use(express.static(__dirname + '/public')); // 将静态文件中间件提到最前面
// 将路由器挂载到应用程序上

app.set('view engine', 'jade');
app.set('views', __dirname + '/public');
app.set('view options', {layout: false});
app.set('basepath',__dirname + '/public');

// 设置环境变量为 development，如果是 production 环境，需要手动设置环境变量为 production
process.env.NODE_ENV = 'development';

// 设置错误处理中间件
app.use(function(err, req, res, next) {
  console.error(err.stack);
  res.status(500).send('Something broke!');
});

console.log("Web server has started.\nPlease log on http://127.0.0.1:3001/index.html");
app.listen(3001);