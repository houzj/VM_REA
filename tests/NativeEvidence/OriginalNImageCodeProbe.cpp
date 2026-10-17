// Controlled direct replay of original tag codeword routine; synthetic bit arrays only.
// No patching, Platform state, device, or product dependency on the original DLL.
#include <windows.h>
#include <cstdio>
#include <vector>
#include <string>
static int mul(int a,int b){int v=0;while(b){if(b&1)v^=a;b>>=1;a<<=1;if(a&256)a^=0x11d;}return v;}
static std::string array(const std::vector<int>& v){std::string s="[";for(size_t i=0;i<v.size();i++){if(i)s+=",";s+=std::to_string(v[i]);}return s+"]";}
int main(int argc,char**argv){SetErrorMode(SEM_FAILCRITICALERRORS|SEM_NOGPFAULTERRORBOX);if(argc!=2)return 2;HMODULE module=LoadLibraryA(argv[1]);if(!module){printf("load %lu",GetLastError());return 3;}auto decode=(int(*)(int*,int,int,int*,void*))((unsigned char*)module+0x8b0260);
 for(int seed=1;seed<=4;seed++){
 std::vector<int> generator(1,1);int alpha=1;for(int k=0;k<18;k++){std::vector<int> next(generator.size()+1);for(size_t i=0;i<generator.size();i++){next[i]^=mul(generator[i],alpha);next[i+1]^=generator[i];}generator=next;alpha=mul(alpha,2);}
 std::vector<int> encoded(24),work(24);for(int i=0;i<6;i++)work[18+i]=encoded[18+i]=(seed*39+i*27)&255;
 for(int i=23;i>=18;i--){int factor=work[i];for(int j=0;j<=18;j++)work[i-18+j]^=mul(factor,generator[j]);}for(int i=0;i<18;i++)encoded[i]=work[i];
 for(int errors=0;errors<=9;errors++){auto input=encoded;for(int i=0;i<errors;i++)input[(i*7+seed)%24]^=(i*13+seed*3)%255+1;std::vector<int> bits(192),output(24,-999);for(int i=0;i<192;i++)bits[i]=(input[i/8]>>(7-i%8))&1;std::vector<unsigned char> scratch(65536);printf("{\"phase\":\"before\",\"seed\":%d,\"errors\":%d}\n",seed,errors);fflush(stdout);int code=decode(bits.data(),18,24,output.data(),scratch.data());printf("{\"seed\":%d,\"errors\":%d,\"code\":%u,\"input\":%s,\"encoded\":%s,\"output\":%s}\n",seed,errors,(unsigned)code,array(input).c_str(),array(encoded).c_str(),array(output).c_str());}
 }printf("{\"phase\":\"finished\",\"coreUnloadSkipped\":true}\n");fflush(stdout);TerminateProcess(GetCurrentProcess(),0);return 0;}
