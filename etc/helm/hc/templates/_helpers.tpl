{{- define "hc.hosts.httpapi" -}}
{{- print "https://" (.Values.global.hosts.httpapi | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "hc.hosts.blazorserver" -}}
{{- print "https://" (.Values.global.hosts.blazorserver | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "hc.hosts.authserver" -}}
{{- print "https://" (.Values.global.hosts.authserver | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
