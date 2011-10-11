import sys
import xml.dom.minidom

def main():
	fname = sys.argv[1]

	success = False
	
	dom = xml.dom.minidom.parse(fname)
	classrefs = {i.getAttribute('Id'):i for i in dom.getElementsByTagName('ComponentRef')}
	components = {i.getAttribute('Id') for i in dom.getElementsByTagName('Component')}
	for id,ref in classrefs.iteritems():
		if id not in components:
			ref.parentNode.removeChild(ref)
	with open(fname,'w') as f:
		f.write(dom.toxml())
	
		

if __name__=='__main__':
	main()
