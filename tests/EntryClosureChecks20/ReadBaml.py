"""Bounded BAML byte records only. No CLR type resolution, schemas or object creation."""
import argparse,hashlib,json,struct
from pathlib import Path
VARIABLE={5,6,15,16,17,18,20,25,27,28,29,30,31,32,36,47,51,52}
FIXED={1:6,2:0,3:3,4:0,7:2,8:0,9:2,10:0,11:2,12:0,13:2,14:0,26:0,33:4,34:4,35:6,37:4,38:8,39:9,40:9,41:0,42:0,43:0,44:2,45:4,46:2,48:3,49:0,50:2,53:8,54:4,55:3,56:4}
def varint(data,p):
 value=0
 for shift in range(0,35,7):
  b=data[p];p+=1;value|=(b&127)<<shift
  if not b&128:return value,p
 raise ValueError('Invalid seven-bit size')
def string(data,p):
 n,p=varint(data,p);return data[p:p+n].decode('utf8'),p+n
def read(path):
 raw=path.read_bytes();size=struct.unpack_from('<I',raw)[0]
 if size!=len(raw)-4:raise ValueError('Resource Stream frame length differs')
 n=struct.unpack_from('<I',raw,4)[0]
 if raw[8:8+n].decode('utf-16-le')!='MSBAML':raise ValueError('Wrong BAML magic')
 p=8+n+12;records=[];types={};attrs={};stack=[];bindings=[]
 while p<len(raw):
  start=p;kind=raw[p];p+=1
  if kind in VARIABLE:
   sizepos=p;size,p=varint(raw,p);end=sizepos+size
  elif kind in FIXED:end=p+FIXED[kind]
  else:raise ValueError(f'Unsupported record {kind} at {start:#x}')
  if end<p or end>len(raw):raise ValueError(f'Record exceeds stream {kind} at {start:#x}')
  r={'offset':hex(start),'kind':kind,'payloadHex':raw[p:end].hex()}
  if kind in (29,30):
   tid,assembly=struct.unpack_from('<hh',raw,p);name,_=string(raw,p+4);types[tid]=name;r.update(typeId=tid,typeName=name)
  elif kind==31:
   aid,owner,usage=struct.unpack_from('<hhB',raw,p);name,_=string(raw,p+5);attrs[aid]={'name':name,'owner':owner};r.update(attributeId=aid,owner=owner,name=name)
  elif kind in (5,36):
   aid=struct.unpack_from('<h',raw,p)[0];value,q=string(raw,p+2);r.update(attributeId=aid,name=attrs.get(aid,{}).get('name',f'known:{aid}'),value=value)
   if kind==36:r['converterTypeId']=struct.unpack_from('<h',raw,q)[0]
  elif kind in (16,17):
   value,q=string(raw,p);r['value']=value
  if kind in (3,48):
   tid=struct.unpack_from('<h',raw,p)[0];element={'offset':hex(start),'typeId':tid,'typeName':types.get(tid,f'known:{tid}'),'properties':[],'nestedText':[]};stack.append(element)
  elif kind in (4,49):
   if not stack:raise ValueError('Unbalanced element end')
   element=stack.pop()
   if stack:stack[-1]['nestedText'].extend(element['nestedText'])
   if any(prop.get('name') in ('Key','Gesture','Modifiers','CommandParameter') for prop in element['properties']):bindings.append(element)
  elif kind in (5,6,33,34,35,36,56) and stack:
   stack[-1]['properties'].append(r)
  elif kind in (16,17) and stack:stack[-1]['nestedText'].append({'offset':hex(start),'value':r['value']})
  records.append(r);p=end
 if stack:raise ValueError('Unclosed element stack')
 return {'path':str(path.resolve()),'sha256':hashlib.sha256(raw).hexdigest(),'recordCount':len(records),'types':types,'attributes':attrs,'bindings':bindings,'records':records}
if __name__=='__main__':
 parser=argparse.ArgumentParser();parser.add_argument('input',type=Path);parser.add_argument('output',type=Path);args=parser.parse_args()
 result=read(args.input);args.output.write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf8');print(json.dumps({'records':result['recordCount'],'bindings':len(result['bindings'])}))
