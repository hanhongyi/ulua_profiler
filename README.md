# ulua_profiler

ulua性能工具:

实现目的:分析unity项目中lua的性能.

实现原理:监视lua函数调用,在调用lua函数时记录时间戳,完成lua函数调用时记录下lua函数的调用时间,同时处理lua中的tail return的情况
在unity项目中使用LuaBigTick 输出每一帧lua的调用情况(可设置大于x毫秒才进行输出)

-- 
ulua profiler :
Purpose: To analyze the performance of lua in the unity project.

Implementation of the principle: hook the lua function call,call timestamp funcion before lua call,complete the lua function whith record time
while dealing with lua in the tail return situation.

in the unity project using LuaBigTick output each frame lua calls(can be set to output while updated more than x milliseconds
