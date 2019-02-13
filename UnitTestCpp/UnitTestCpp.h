// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the UNITTESTCPP_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// UNITTESTCPP_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef UNITTESTCPP_EXPORTS
#define UNITTESTCPP_API __declspec(dllexport)
#else
#define UNITTESTCPP_API __declspec(dllimport)
#endif

