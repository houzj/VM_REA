// Offline instruction-order model of 7fec40 copy/sort only. Never calls original DLL.
// Every logical negative access is bounded by the captured prefix; overflow is failure.
#include <cstdio>
#include <vector>
#include <cstring>
#include <stdexcept>
#include <cstdint>
static unsigned Bits(float v){unsigned b;memcpy(&b,&v,4);return b;}
struct Trace {
 std::vector<float> buffer; int offset,n,reads=0,writes=0,minRead=0,minWrite=0,firstReadI=-1,firstWriteI=-1;
 float sigma=0; int pairs=0,tail=0;
 float read(int j,int i){if(j<0){reads++;if(firstReadI<0)firstReadI=i;if(j<minRead)minRead=j;}if(offset+j<0||offset+j>=(int)buffer.size())throw std::runtime_error("captured prefix bound crossed");return buffer[offset+j];}
 void write(int j,float v,int i){if(j<0){writes++;if(firstWriteI<0)firstWriteI=i;if(j<minWrite)minWrite=j;}if(offset+j<0||offset+j>=(int)buffer.size())throw std::runtime_error("captured prefix bound crossed");buffer[offset+j]=v;}
 Trace(const std::vector<float>&errors,const std::vector<float>&prefix):buffer(prefix),offset((int)prefix.size()),n((int)errors.size()){
  if(n<1)throw std::runtime_error("offline model accepts positive counts only");buffer.resize(offset+n);
  auto copy=[&](int k){float v=errors[k];if(0.0f>v){unsigned b=Bits(v)^0x80000000u;memcpy(&v,&b,4);}write(k,v,-1);};
  pairs=n/2;for(int k=0;k<pairs;k++){copy(2*k);copy(2*k+1);}tail=n&1;if(tail)copy(n-1);
  for(int i=0;i<n-1;i++){float key=read(i+1,i),previous=read(i,i);if(!(previous>key))continue;int j=i;
   do{write(j+1,read(j,i),i);--j;}while(read(j,i)>key);write(j+1,key,i);
  }
  float multiplier;unsigned b=0x3fbdc52c;memcpy(&multiplier,&b,4);sigma=read(n/2,n)*multiplier;
 }
 void print(int seq,bool captured,bool originalSortedMatch){printf("{\"sequence\":%d,\"captured\":%s,\"count\":%d,\"pairs\":%d,\"tail\":%d,\"medianIndex\":%d,\"sigmaBits\":%u,\"negativeReads\":%d,\"negativeWrites\":%d,\"minRead\":%d,\"minWrite\":%d,\"firstNegativeReadOuterIndex\":%d,\"firstNegativeWriteOuterIndex\":%d,\"nativeCapturedSortedMatch\":%s}\n",seq,captured?"true":"false",n,pairs,tail,n/2,Bits(sigma),reads,writes,minRead,minWrite,firstReadI,firstWriteI,originalSortedMatch?"true":"false");}
};
int main(int argc,char**argv){if(argc!=2)return 2;try{
 for(int seq:{4,56,63,67,76,83}){char path[4096];sprintf_s(path,"%s/median-entry-%03d-00.bin",argv[1],seq);FILE*f=fopen(path,"rb");if(!f)throw std::runtime_error("entry missing");int n,freedom,prefix;fread(&n,4,1,f);fread(&freedom,4,1,f);fread(&prefix,4,1,f);fseek(f,32+16*n,SEEK_CUR);std::vector<float> history(prefix);if(fread(history.data(),4,prefix,f)!=prefix)throw std::runtime_error("short prefix");fclose(f);
 sprintf_s(path,"%s/median-%03d-00.bin",argv[1],seq);f=fopen(path,"rb");if(!f)throw std::runtime_error("median output missing");std::vector<float> errors(n),sorted(n);if(fread(errors.data(),4,n,f)!=n||fread(sorted.data(),4,n,f)!=n)throw std::runtime_error("short output");fclose(f);
 for(bool capture:{false,true}){Trace t(errors,capture?history:std::vector<float>(prefix,0));bool match=!memcmp(t.buffer.data()+t.offset,sorted.data(),n*4);if(capture&&!match)throw std::runtime_error("captured sorted bytes differ");t.print(seq,capture,match);}
 }
 // Independent branch witnesses, not original executions or runtime parity claims.
 int seq=-1;for(auto errors:{std::vector<float>{3},std::vector<float>{3,1,2},std::vector<float>{4,3,2,1},std::vector<float>{3,2,1}}){std::vector<float> prefix(4,0);if(seq==-4)prefix.back()=5;Trace t(errors,prefix);t.print(seq--,false,false);}
 return 0;
 }catch(const std::exception&e){fprintf(stderr,"%s\n",e.what());return 3;}}
