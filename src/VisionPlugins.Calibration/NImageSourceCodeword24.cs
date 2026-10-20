namespace VisionPlugins.Calibration;

// Source translation for the fixed 24-symbol product caller; see R03 implementation evidence.
// Core SHA256 85d17ecc87d52e9a62d8bb86c8d5208ac999bcd31ec6d0806453f8165733f549.
// Each descriptor below stores native coefficient order and the low 32-bit degree.
// SIMD/alias branches are replaced by independent managed buffers for this fixed caller.
internal static class NImageSourceCodeword24 {
 internal sealed record Result(uint? NativeStatus,byte[] Symbols,string? UnsupportedReason=null) {
  internal bool Success=>NativeStatus==0&&UnsupportedReason is null;
 }
 readonly record struct Poly(int[] Words,int Offset,int Degree) {
  internal int Get(int i)=>Words[Offset+i];
  internal void Set(int i,int value)=>Words[Offset+i]=value;
 }
 static readonly int[] Exp=new int[256];
 // PE .data virtual-tail zero-fill establishes Log[0]=0; 932480 preserves that slot.
 // Current 8b0260 chain writes only nonzero GF indices in 932480.
 static readonly int?[] Log=new int?[256];
 static NImageSourceCodeword24(){
  Log[0]=0; // Same-hash PE RVA 2fcc4c0 loader initialization, not a generic GF rule.
  Exp[0]=1;Log[1]=0;int value=2;
  for(int i=1;i<256;i++){Exp[i]=value;Log[value]=i;value=Mul(value,2);}
 }
 static int Mul(int a,int b){ // 932080 / inlined 0x11d reduction, byte-domain inputs only.
  int product=0;for(int shift=0;b!=0;shift++,b>>=1)if((b&1)!=0)product^=a<<shift;
  for(int bit=15;bit>=8;bit--)if((product&(1<<bit))!=0)product^=0x11d<<(bit-8);
  return product;
 }
 static int Alpha(int exponent){int value=1;for(int i=0;i<exponent;i++)value=Mul(value,2);return value;} // 932370
 static int Evaluate(Poly p,int x){ // 9322a0: start last coefficient, walk downward.
  int value=p.Get(p.Degree);for(int i=p.Degree-1;i>=0;i--)value=Mul(value,x)^p.Get(i);return value;
 }
 static int InverseNonzero(int value){ // 85aee0: Exp[255-Log[value]], not a generic zero inverse.
  if(value==0||Log[value] is not int log)throw new InvalidOperationException("Unverified Log[0] access");
  return Exp[255-log];
 }
 static int PowerNonzero(int value,int power)=>Exp[(power*Log[value]!.Value)%255]; // 932440
 static void Scale(Poly input,int scalar,Poly output){ // 931cd0, does not clear the unused tail.
  for(int i=0;i<=input.Degree;i++)output.Set(i,Mul(input.Get(i),scalar));
 }
 static void EndAlignedXor(Poly a,Poly b,Poly output){ // 931dd0, preserves untouched output prefix.
  int length=Math.Max(a.Degree+1,b.Degree+1);
  for(int i=0;i<=a.Degree;i++)output.Set(length-a.Degree-1+i,a.Get(i));
  for(int i=0;i<=b.Degree;i++){int index=length-b.Degree-1+i;output.Set(index,output.Get(index)^b.Get(i));}
 }
 static void Multiply(Poly a,Poly b,Poly output){ // 932120: clears exactly sumDegrees+1 coefficients.
  int count=a.Degree+b.Degree+1;if(count>0)Array.Clear(output.Words,output.Offset,count);
  for(int i=0;i<=a.Degree;i++)for(int j=0;j<=b.Degree;j++)output.Set(i+j,output.Get(i+j)^Mul(a.Get(i),b.Get(j)));
 }
 static uint Reverse(Poly a,Poly output){ // 932570, both backing pointers are nonnull managed arrays.
  if(a.Degree!=output.Degree)return 0x10000019;
  for(int i=0;i<=a.Degree;i++)output.Set(a.Degree-i,a.Get(i));return 0;
 }
 static void Copy(Poly input,Poly output,int count)=>Array.Copy(input.Words,input.Offset,output.Words,output.Offset,count);

 internal static Result Correct(ReadOnlySpan<byte> symbols){
  // Independent safe managed-entry guard. Native fixed-count pointers have no span length.
  if(symbols.Length!=24)return new(null,[],"Only the actual 24-symbol caller is supported");
  var original=symbols.ToArray().Select(x=>(int)x).ToArray();var syndrome=new int[32];
  var word=new Poly(original,0,23);
  for(int i=0;i<18;i++)syndrome[i]=Evaluate(word,Alpha(i)); // 8b0260
  if(syndrome.Take(18).All(x=>x==0))return new(0,symbols.ToArray());
  var locations=FindLocations(syndrome);if(locations.Status!=0)return new(locations.Status,[]);
  return ApplyMagnitudes(syndrome,locations.Positions,original);
 }

 static (uint Status,int[] Positions) FindLocations(int[] syndrome){ // 8b11a0 fixed checkCount=18
  // Native R=align128((18+1)*4)=128 bytes. Six persistent regions; zeroed once by 8b0260.
  const int stride=32;var arena=new int[6*stride];
  for(int i=0;i<18;i++)arena[i+1]=syndrome[i];
  int cOffset=stride,swapOffset=2*stride,bOffset=3*stride,scaledOffset=4*stride,xorOffset=5*stride;
  arena[cOffset]=arena[bOffset]=1;int cDegree=0,bDegree=0;
  for(int n=0;n<18;n++){
   int discrepancy=arena[n+1];
   for(int j=1;j<=cDegree;j++)discrepancy^=Mul(arena[cOffset+cDegree-j],arena[n+1-j]);
   // Native advances the descriptor, WITHOUT writing a new zero coefficient.
   bDegree++;
   if(discrepancy==0)continue;
   if(cDegree<bDegree){
    Scale(new(arena,bOffset,bDegree),discrepancy,new(arena,swapOffset,bDegree));
    Scale(new(arena,cOffset,cDegree),InverseNonzero(discrepancy),new(arena,bOffset,cDegree));
    int oldC=cDegree;cDegree=bDegree;bDegree=oldC;
    Copy(new(arena,swapOffset,cDegree),new(arena,cOffset,cDegree),cDegree+1);
   }
   Scale(new(arena,bOffset,bDegree),discrepancy,new(arena,scaledOffset,bDegree));
   int nextDegree=Math.Max(cDegree,bDegree);
   EndAlignedXor(new(arena,cOffset,cDegree),new(arena,scaledOffset,bDegree),new(arena,xorOffset,nextDegree));
   cDegree=nextDegree;Copy(new(arena,xorOffset,cDegree),new(arena,cOffset,cDegree),cDegree+1);
  }
  var positions=new List<int>();for(int i=0;i<24;i++)if(Evaluate(new(arena,cOffset,cDegree),Alpha(i))==0)positions.Add(i);
  // No independent degree 1..9 gate: preserve the native roots == descriptor degree predicate.
  return positions.Count==cDegree?(0,positions.ToArray()):(unchecked((uint)-1000),[]);
 }

 static Result ApplyMagnitudes(int[] syndrome,int[] positions,int[] original){ // 8b0670
  int count=positions.Length;var locator=new int[64];locator[0]=1;
  var linear=new int[64];var product=new int[64];var factor=new int[64];var one=new int[64];one[0]=1;
  int degree=0;
  foreach(int position in positions){
   factor[0]=Alpha(position); // factor[1] stays the source-initialized zero.
   EndAlignedXor(new(factor,0,1),new(one,0,0),new(linear,0,1));
   Multiply(new(locator,0,degree),new(linear,0,1),new(product,0,degree+1));degree++;
   Copy(new(product,0,degree),new(locator,0,degree),degree+1);
  }
  var locatorCopy=new int[64];Array.Copy(locator,locatorCopy,degree+1);
  var reversedSyndrome=new int[64];
  uint firstReverse=Reverse(new(syndrome,0,count-1),new(reversedSyndrome,0,count-1));
  var extracted=new int[64];
  // This buffer corresponds to uStack_218, which held the locator before convolution.
  // Preserve its initialized contents for the count==0 zero-byte-clear branch.
  var convolution=new int[64];Array.Copy(locator,convolution,degree+1);
  if(firstReverse==0){
   Multiply(new(locatorCopy,0,count),new(reversedSyndrome,0,count-1),new(convolution,0,2*count-1));
   // Native copies count+1 terms starting at coefficient count, including its zero workspace tail.
   Array.Copy(convolution,count,extracted,0,count+1);
  }
  var xPositions=positions.Select(p=>PowerNonzero(2,p)).ToArray();
  var omega=new int[64];uint secondReverse=Reverse(new(extracted,0,count),new(omega,0,count));
  if(secondReverse!=0)return new(secondReverse,[]);
  var mask=new int[24];
  for(int i=0;i<count;i++){
   int inverse=InverseNonzero(xPositions[i]),denominator=1;
   for(int j=0;j<count;j++)if(j!=i)denominator=Mul(denominator,Mul(inverse,xPositions[j])^1);
   int numerator=Mul(PowerNonzero(xPositions[i],1),Evaluate(new(omega,0,count),inverse));
   if(denominator==0)return new(0x10000019,[]); // 931c60 failure before final 24-word xor.
   // 931c60 writes zero for numerator==0, then FALLS THROUGH and overwrites it.
   // Preserve that lookup with PE-initialized Log[0]=0; do not short-circuit to zero.
   int magnitude=Exp[(Log[numerator]!.Value-Log[denominator]!.Value+255)%255];
   mask[positions[i]]=magnitude;
  }
  var corrected=new byte[24];for(int i=0;i<24;i++)corrected[i]=(byte)(original[i]^mask[i]);
  // No new syndrome acceptance gate: native returns immediately after writing these 24 words.
  return new(0,corrected);
 }
}
