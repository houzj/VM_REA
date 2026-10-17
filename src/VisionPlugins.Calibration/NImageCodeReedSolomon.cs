namespace VisionPlugins.Calibration;

/// <summary>Independent GF(256) codeword correction from original 8b0260/932480.
/// Symbols are ascending polynomial coefficients; 18 syndromes alpha^0..alpha^17.
/// This is the tag codeword layer, not an image detector.</summary>
public static class NImageCodeReedSolomon {
 static readonly int[] Exp=new int[510],Log=new int[256];
 static NImageCodeReedSolomon(){int value=1;for(int i=0;i<255;i++){Exp[i]=value;Log[value]=i;value<<=1;if((value&256)!=0)value^=0x11d;}for(int i=255;i<510;i++)Exp[i]=Exp[i-255];}
 static int Mul(int a,int b)=>a==0||b==0?0:Exp[Log[a]+Log[b]];
 static int Div(int a,int b)=>b==0?throw new InvalidOperationException("Zero field divisor"):a==0?0:Exp[(Log[a]-Log[b]+255)%255];
 static int Evaluate(ReadOnlySpan<int> polynomial,int x){int value=0;for(int i=polynomial.Length-1;i>=0;i--)value=Mul(value,x)^polynomial[i];return value;}
 public static bool TryCorrect(ReadOnlySpan<byte> symbols,out byte[] corrected){
  corrected=[];if(symbols.Length!=24)return false;int[] word=symbols.ToArray().Select(v=>(int)v).ToArray();int[] syndrome=new int[18];for(int i=0;i<18;i++)syndrome[i]=Evaluate(word,Exp[i]);if(syndrome.All(v=>v==0)){corrected=symbols.ToArray();return true;}
  int[] locator=new int[19],previous=new int[19];locator[0]=previous[0]=1;int degree=0,shift=1,last=1;
  for(int n=0;n<18;n++){int discrepancy=syndrome[n];for(int i=1;i<=degree;i++)discrepancy^=Mul(locator[i],syndrome[n-i]);if(discrepancy==0){shift++;continue;}var saved=(int[])locator.Clone();int factor=Div(discrepancy,last);for(int i=0;i+shift<locator.Length;i++)locator[i+shift]^=Mul(factor,previous[i]);if(2*degree<=n){degree=n+1-degree;previous=saved;last=discrepancy;shift=1;}else shift++;}
  if(degree is <1 or >9)return false;var positions=new List<int>();for(int i=0;i<24;i++)if(Evaluate(locator,Exp[(255-i)%255])==0)positions.Add(i);if(positions.Count!=degree)return false;
  // Solve magnitudes directly from the first syndromes. This avoids importing native scratch layout.
  int[,] system=new int[degree,degree+1];for(int r=0;r<degree;r++){for(int c=0;c<degree;c++)system[r,c]=Exp[(positions[c]*r)%255];system[r,degree]=syndrome[r];}
  for(int c=0;c<degree;c++){int pivot=c;while(pivot<degree&&system[pivot,c]==0)pivot++;if(pivot==degree)return false;if(pivot!=c)for(int k=c;k<=degree;k++)(system[c,k],system[pivot,k])=(system[pivot,k],system[c,k]);int divisor=system[c,c];for(int k=c;k<=degree;k++)system[c,k]=Div(system[c,k],divisor);for(int r=0;r<degree;r++)if(r!=c){int factor=system[r,c];for(int k=c;k<=degree;k++)system[r,k]^=Mul(factor,system[c,k]);}}
  for(int i=0;i<degree;i++)word[positions[i]]^=system[i,degree];for(int i=0;i<18;i++)if(Evaluate(word,Exp[i])!=0)return false;corrected=word.Select(v=>(byte)v).ToArray();return true;
 }
 public static bool TryDecodeBits(ReadOnlySpan<byte> bits,out byte[] exportedBytes){exportedBytes=[];if(bits.Length!=192)return false;byte[] symbols=new byte[24];for(int i=0;i<192;i++){if(bits[i]>1)return false;symbols[i/8]|=(byte)(bits[i]<<(7-i%8));}if(!TryCorrect(symbols,out var corrected))return false;exportedBytes=Enumerable.Range(0,12).Select(i=>corrected[23-i]).ToArray();return true;}
}
