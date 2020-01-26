# Fpoke-CLI
CLI written in F# to check if a website is up or not.

## How to use
Simply input  `fpoke` and the url you'd like to check.

Example:
 `fpoke https://google.com/`
 
### Send notification emails
You can also send an email with the result by adding the -email parameter. You'll need to setup the connection to the SMTP server in the SmtpConnections.xml.

Example:
`fpoke https://google.com/ -email youremail@domain.com`

If you're like most people and don't have a dedicated SMTP server on hand, you can use [Google's SMTP server](https://www.digitalocean.com/community/tutorials/how-to-use-google-s-smtp-server) with a google account.

### Check ports
You can check an open port with the -p parameter.

Example:
`fpoke https://google.com/ -p 80`

### Respond only to errors
If you only want a notification only when there's an error, you can use the -error parameter. This is great for automating a check to see if a website is down.

Example:
`fpoke https://google.com/ -email youremail@domain.com -error`

### Check status of all urls
With the -list parameter, you can check the status of all urls within the urls.json file.

Example:
`fpoke -list`
