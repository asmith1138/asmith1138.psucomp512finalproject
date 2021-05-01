
server {

        #root /var/www/comp512.smithions.com/html;
        #index index.html index.htm index.nginx-debian.html;

        server_name comp512.smithions.com www.comp512.smithions.com;

	location / {
                proxy_pass https://localhost:5001;
                proxy_http_version 1.1;
                proxy_set_header Upgrade $http_upgrade;
                proxy_set_header Connection "upgrade";
                proxy_set_header Host $host;
                proxy_cache_bypass $http_upgrade;
                proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
                proxy_set_header X-Forwarded-Proto $scheme;
        }
	location /chat {
		proxy_pass https://localhost:5001;
		proxy_http_version 1.1;
		proxy_set_header Upgrade $http_upgrade;
		proxy_set_header Connection "upgrade";
		proxy_cache off;
		proxy_buffering off;
		proxy_read_timeout 100s;
		proxy_set_header Host $host;
		proxy_cache_bypass $http_upgrade;
		proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
		proxy_set_header X-Forwarded-Proto $scheme;
	}
        #location / {
        #        try_files $uri $uri/ =404;
        #}

    listen [::]:443 ssl ipv6only=on; # managed by Certbot
    listen 443 ssl; # managed by Certbot
    ssl_certificate /etc/letsencrypt/live/comp512.smithions.com/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/comp512.smithions.com/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem; # managed by Certbot


}
server {
    if ($host = www.comp512.smithions.com) {
        return 301 https://$host$request_uri;
    } # managed by Certbot


    if ($host = comp512.smithions.com) {
        return 301 https://$host$request_uri;
    } # managed by Certbot


        listen 80;
        listen [::]:80;

        server_name comp512.smithions.com www.comp512.smithions.com;
    return 404; # managed by Certbot




}

