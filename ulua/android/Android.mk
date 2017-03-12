LOCAL_PATH := $(call my-dir)
include $(CLEAR_VARS)

LOCAL_MODULE := ulua
LOCAL_CFLAGS := -fexceptions -O2
LOCAL_LDFLAGS := -Xlinker -version-script=ldscript
LOCAL_LDLIBS := \
	-llog \
	-lz \
	-landroid \

LOCAL_SRC_FILES := \
	../cjson/dtoa.c \
	../cjson/fpconv.c \
	../cjson/g_fmt.c \
	../cjson/lua_cjson.c \
	../cjson/strbuf.c \
	../lua/clocks.c \
	../lua/lapi.c \
	../lua/lauxlib.c \
	../lua/lbaselib.c \
	../lua/lcode.c \
	../lua/ldblib.c \
	../lua/ldebug.c \
	../lua/ldo.c \
	../lua/ldump.c \
	../lua/lfunc.c \
	../lua/lgc.c \
	../lua/linit.c \
	../lua/liolib.c \
	../lua/llex.c \
	../lua/lmathlib.c \
	../lua/lmem.c \
	../lua/loadlib.c \
	../lua/lobject.c \
	../lua/lopcodes.c \
	../lua/loslib.c \
	../lua/lparser.c \
	../lua/lstate.c \
	../lua/lstring.c \
	../lua/lstrlib.c \
	../lua/ltable.c \
	../lua/ltablib.c \
	../lua/ltm.c \
	../lua/lundump.c \
	../lua/lvm.c \
	../lua/lzio.c \
	../lua/print.c \
	../lua_wrap.c \
	../socket/auxiliar.c \
	../socket/buffer.c \
	../socket/compat.c \
	../socket/except.c \
	../socket/inet.c \
	../socket/io.c \
	../socket/luasocket.c \
	../socket/mime.c \
	../socket/options.c \
	../socket/select.c \
	../socket/serial.c \
	../socket/tcp.c \
	../socket/timeout.c \
	../socket/udp.c \
	../socket/unix.c \
	../socket/usocket.c \
	../socket/wsocket.c \

LOCAL_C_INCLUDES += ../lua
LOCAL_C_INCLUDES += ../cjson
LOCAL_C_INCLUDES += ../socket

include $(BUILD_SHARED_LIBRARY)
