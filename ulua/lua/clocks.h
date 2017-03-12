/*
** LuaProfiler
** Copyright Kepler Project 2005-2007 (http://www.keplerproject.org/luaprofiler)
** $Id: clocks.h,v 1.4 2007-08-22 19:23:53 carregal Exp $
*/

/*****************************************************************************
clocks.h:
   Module to register the time (seconds) between two events

Design:
   'lprofC_start_timer()' marks the first event
   'lprofC_get_seconds()' gives you the seconds elapsed since the timer
                          was started
*****************************************************************************/

#include <time.h>
#include "def.h"

void lprofC_start_timer(lprof_clock *time_marker);
lprof_clock lprofC_get_seconds(lprof_clock time_marker);
