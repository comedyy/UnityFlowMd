```mermaid
flowchart LR
A(Hard)
B[Round]
C{Condition}
H((inputoutput))
D[Result 1]
E[Result 2]
G([End])

A --> B --> C --> D
C -->|NO| E-->H-->G
D -->G
```